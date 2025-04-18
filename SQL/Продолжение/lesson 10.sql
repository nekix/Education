-- Задача 2*: Комплексный анализ эффективности производства

/* Возвращаемый REST
[
  {
    "workshop_id": 301,
    "workshop_name": "Royal Forge",
    "workshop_type": "Smithy",
    "num_craftsdwarves": 4,
    "total_quantity_produced": 256,
    "total_production_value": 187500,
    
    "daily_production_rate": 3.41,
    "value_per_material_unit": 7.82,
    "workshop_utilization_percent": 85.33,
    
    "material_conversion_ratio": 1.56,
    
    "average_craftsdwarf_skill": 7.25,
    
    "skill_quality_correlation": 0.83,
    
    "related_entities": {
      "craftsdwarf_ids": [101, 103, 108, 115],
      "product_ids": [801, 802, 803, 804, 805, 806],
      "material_ids": [201, 204, 208, 210],
      "project_ids": [701, 702, 703]
    }
  },
  {
    "workshop_id": 304,
    "workshop_name": "Gemcutter's Studio",
    "workshop_type": "Jewelcrafting",
    "num_craftsdwarves": 2,
    "total_quantity_produced": 128,
    "total_production_value": 205000,
    
    "daily_production_rate": 2.56,
    "value_per_material_unit": 13.67,
    "workshop_utilization_percent": 78.95,
    
    "material_conversion_ratio": 0.85,
    
    "average_craftsdwarf_skill": 8.50,
    
    "skill_quality_correlation": 0.92,
    
    "related_entities": {
      "craftsdwarf_ids": [105, 112],
      "product_ids": [820, 821, 822, 823, 824],
      "material_ids": [206, 213, 217, 220],
      "project_ids": [705, 708]
    }
  }
]
*/

-- Запрос

WITH dwarves_crafts_skills AS (
    SELECT
        ds.dward_id,
        ds.skill_id,
        ds.level,
        ds.date,
        -- Захватываю следующую дату, позволяет определить, самая актуальная это 
        -- запись по данном дворфу и скилу, или нет (или определить была ли она
        -- актуальной в нужный период).
        LEAD(ds.date, 1, NULL) OVER (PARTITION BY ds.dward_id, ds.skill_id ORDER BY ds.date) AS next_date 
    FROM
        dwarf_skills AS ds
    INNER JOIN
    -- Предположил, что скилы бывают разные и это как способ получить
    -- связанные именно с ремеслом
        skills AS s ON s.skill_id = ds.skill_id
            AND s.category = 'Crafts'
),
all_dwarfs_skills_products_stats AS (
    SELECT
        wp.workshop_id,
        SUM(wp.quantity) AS quantity,
        p.quality,
        wp.production_date,
        SUM(wp.quantity) * p.value AS production_value,
        COALESCE(SUM(dcs.level), 0) AS dwarfs_skills_levels_sum
    FROM
        workshop_products AS wp
    INNER JOIN
        products AS p ON p.product_id = wp.product_id
    LEFT OUTER JOIN
        -- принял, хотя в таблице прямо не указана (FK) и в описании нет, 
        -- но как будто подразумевается, products.created_by - это dwarf_id, 
        -- который его изготовил. Иначе не понимаю, как проследить связь для статистики
        dwarves_crafts_skills AS dcs ON dcs.dward_id = p.created_by -- как раз определение актуальности за период
            AND dcs.date <= wp.production_date AND dcs.next_date > wp.production_date
    GROUP BY
        wp.workshop_id,
        wp.product_id,
        wp.production_date,
        p.quality,
        p.value
),
-- шаг, чтобы избежать группировок и суммирований в основном запросе
product_stats AS (
    SELECT
        workshop_id
        SUM(quantity) AS total_quantity_produced
        SUM(production_value) AS total_production_value,
        MIN(production_date) AS start_production_date
    FROM
        all_dwarfs_skills_products_stats
    GROUP BY
        workshop_id
),
-- подготовка данных к расчёту корреляции скилов и качества продукции
workshop_skill_quality_stats AS (
    SELECT
        workshop_id,
        SUM(dwarfs_skills_levels_sum) AS total_skills_level,
        AVG(quality) AS avg_date_quality
    FROM
        all_dwarfs_skills_products_stats
    GROUP BY
        workshop_id, 
        production_date -- важная группировка, считает по набору с разбивкой по дням
),
-- расчёт коэффициента корреляции Пирсона
workshop_skill_quality_correletation AS (
    SELECT
        workshop_id,
        (SUM(avg_date_quality * total_skills_level) 
            - SUM(avg_date_quality) * SUM(total_skills_level) / NULLIF(COUNT (avg_date_quality), 0)
        ) / NULLIF(COUNT(avg_date_quality), 0) 
        / NULLIF((STDEV(avg_date_quality), 0) * STDEV(total_skills_level), 0)       
        AS skill_quality_correlation
    FROM
        workshop_skill_quality_stats
    GROUP BY
        workshop_id
),
used_materials_stats AS (
    SELECT
        workshop_id,
        SUM(quantity) AS total_quantity
    FROM
        workshop_material
    WHERE 
        is_input = true
    GROUP BY
        workshop_id
),
current_dwarves_stats(
    SELECT
        wc.workshop_id,
        COUNT(wc.dwarf_id) AS num_craftsdwarves,
        SUM(ds.level) AS total_level,
        JSON_ARRAYAGG(wc.dward_id) AS dwarf_ids_json
    FROM
        workshop_craftsdwarves AS wc
    INNER JOIN -- как раз определение актуального скила
        dwarves_crafts_skills AS ds ON wc.dward_id = ds.dward_id AND wc.next_date IS NULL 
    GROUP BY
        wc.workshop_id
)

SELECT
    w.workshop_id,
    w.name AS workshop_name,
    w.type AS workshop_type,
    COALESCE(cds.num_craftsdwarves, 0),
    COALESCE(ps.total_quantity_produced, 0),
    COALESCE(ps.total_production_value, 0),
    
    -- Принял что мастерская работает со дня изготовления первого продукта
    -- получаю среднюю норму в день за всё время (до текущей даты).
    COALESCE(ROUND(ps.total_quantity_produced / NULLIF(DATEDIFF(ps.start_production_date - NOW()), 0), 2), 0) AS daily_production_rate,
    COALESCE(ROUND(ps.total_production_value / NULLIF(mat.total_quantity, 0), 2), 0) AS value_per_material_unit,
    
    -- Не смог реализовать вычисление workshop_utilization_percent
    -- Я не понимаю как можно вычислить загруженность мастерской, в таблицах будто бы
    -- недостаточно для этого данных, потому что у нас нет истории работы мастерской,
    -- есть именно история произведенных ею товаров, что косвенный признак, ведь например
    -- мастерская могла быть загружена производством долгих товаров или работала над проектом.
    -- Будет очень интересно посмотреть на эталонное решение, узнать где я мог это упустить.
    -- ... AS workshop_utilization_percent,
    
    COALESCE(ROUND(mat.total_quantity / NULLIF(ps.total_quantity_produced, 0), 2), 0) AS material_conversion_ratio,
    COALESCE(ROUND(cds.total_level / NULLIF(num_craftsdwarves, 0), 2), 0) AS average_craftsdwarf_skill,
    -- Вычислял группируя по дням производства продукции, забирая сумму уровней гномов в этот день 
    -- и средннее качество продукции за день
    COALESCE(ROUND(sq_corr.skill_quality_correlation, 2), 0),
    
    JSON_OBJECT(
        -- Подумал что раз мы уже захватили нужную таблицу в CTE,
        -- то почему бы сразу не забрать и эти данные.
        'craftsdwarf_ids', cds.dwarf_ids_json,
        'product_ids', (
            SELECT JSON_ARRAYAGG(wp.product_id)
            FROM workshop_products wp
            WHERE wp.workshop_id = w.workshop_id
        ),
        'material_ids', (
            SELECT JSON_ARRAYAGG(wm.material_id)
            FROM workshop_materials wm
            WHERE wm.workshop_id = w.workshop_id
        ),
        'project_ids' (
            SELECT JSON_ARRAYAGG(pr.project_id)
            FROM projects pr
            WHERE pr.workshop_id = w.workshop_id
        )
    ) AS related_entities
FROM
    workshops w
LEFT OUTER JOIN
    product_stats AS ps ON ps.workshop_id = w.workshop_id
LEFT OUTER JOIN
    workshop_skill_quality_correletation AS sq_corr ON sq_corr.workshop_id = w.workshop_id
LEFT OUTER JOIN
    used_materials_stats AS mat ON mat.workshop_id = w.workshop_id
LEFT OUTER JOIN
    current_dwarves_stats AS cds ON cds.workshop_id = w.workshop_id  