-- Задача 1*: Анализ эффективности экспедиций

/* Возвращаемый REST
[
  {
    "expedition_id": 2301,
    "destination": "Ancient Ruins",
    "status": "Completed",
    "survival_rate": 71.43,
    "artifacts_value": 28500,
    "discovered_sites": 3,
    "encounter_success_rate": 66.67,
    "skill_improvement": 14,
    "expedition_duration": 44,
    "overall_success_score": 0.78,
    "related_entities": {
      "member_ids": [102, 104, 107, 110, 112, 115, 118],
      "artifact_ids": [2501, 2502, 2503],
      "site_ids": [2401, 2402, 2403]
    }
  },
  {
    "expedition_id": 2305,
    "destination": "Deep Caverns",
    "status": "Completed",
    "survival_rate": 80.00,
    "artifacts_value": 42000,
    "discovered_sites": 2,
    "encounter_success_rate": 83.33,
    "skill_improvement": 18,
    "expedition_duration": 38,
    "overall_success_score": 0.85,
    "related_entities": {
      "member_ids": [103, 105, 108, 113, 121],
      "artifact_ids": [2508, 2509, 2510, 2511],
      "site_ids": [2410, 2411]
    }
  },
  {
    "expedition_id": 2309,
    "destination": "Abandoned Fortress",
    "status": "Completed",
    "survival_rate": 50.00,
    "artifacts_value": 56000,
    "discovered_sites": 4,
    "encounter_success_rate": 42.86,
    "skill_improvement": 23,
    "expedition_duration": 62,
    "overall_success_score": 0.63,
    "related_entities": {
      "member_ids": [106, 109, 111, 119, 124, 125],
      "artifact_ids": [2515, 2516, 2517, 2518, 2519],
      "site_ids": [2420, 2421, 2422, 2423]
    }
  }
]
*/

-- Запрос
SELECT
    agg.*,
    (1.0 * agg.survival_rate / MAX(agg.survival_rate)
        + 1.0 * agg.artifacts_value / MAX(agg.artifacts_value)
        + 1.0 * agg.discovered_sites / MAX(agg.discovered_sites)
        + 1.0 * agg.encounter_success_rate / MAX(agg.encounter_success_rate)
        + 1.0 * agg.skill_improvement / MAX(agg.skill_improvement))
        / 5.0                                                               AS overall_success_score
-- Для вычисления overall_success_score пришлось все основные агрегации перенести в подзапрос,
-- потому что мы тут по факту нормализуем данные из которых считается overall_success_score от 0 до 1.
-- Если бы были требования о весе каждого параметра, то можно было бы изменить соотвественно коэффициенты
-- 1.0 и 5.0 в формуле.
FROM (
    SELECT 
        e.expedition_id                                                     AS expedition_id,
        e.destination                                                       AS destination,
        e.status                                                            AS status,
        COUNT(CASE WHEN em.survived THEN 1 END) / COUNT(*) * 100            AS survival_rate,
        SUM(ea.value)                                                       AS artifacts_value,
        COUNT(DISTINCT(es.site_id))                                         AS discovered_sites,
        COUNT(CASE WHEN ec.outcome = 'Success' THEN 1 END) / COUNT(*) * 100 AS encounter_success_rate,
        SUM(de_after.experience) - SUM(de_before.experience)                AS skill_improvement
        DATEDIFF(e.return_date - e.departure_date)                          AS expedition_duration,
        JSON_OBJECT(
            'member_ids',   JSON_ARRAYAGG(em.member_id)
            'artifact_ids', JSON_ARRAYAGG(ea.artifact_id),
            'site_ids',     JSON_ARRAYAGG(es.site_id))                      AS related_entities
    FROM expeditions e
    -- Для повышения читаемости выбрал JOIN-ы
    -- (иначе могло бы быть много вложенных функций)
    LEFT OUTER JOIN expedition_members              AS em       ON em.expedition_id = e.expedition_id 
    LEFT OUTER JOIN expedition_artifacts            AS ea       ON ea.expedition_id = e.expedition_id
    LEFT OUTER JOIN expedition_sites                AS es       ON es.expedition_id = e.expedition_id
    LEFT OUTER JOIN expedition_creatures            AS ec       ON ec.expedition_id = e.expedition_id
    -- Здесь мы сначала берем все навыки, которые получили
    -- прогресс в период похода
    LEFT OUTER JOIN (
            SELECT ds.dwarf_id, SUM(ds.experience) AS experience
            FROM dwarf_skills ds
            WHERE ds.date = (
                    SELECT MAX(date) 
                    FROM dwarf_skills 
                    WHERE dwarf_id = ds.dwarf_id 
                        AND skill_id = ds.skill_id 
                        AND date < e.return_date)
            GROUP BY ds.dwarf_id)                   AS de_after ON de_after.dwarf_id = em.dwarf_id
    -- А затем добавляем (если есть), последние до похода значения 
    -- прогресса по этим навыкам. Если их нет, значит прогресс был 0.
    LEFT OUTER JOIN (
            SELECT ds.dwarf_id, SUM(ds.experience) AS experience
            FROM dwarf_skills ds
            WHERE ds.date = (
                    SELECT MAX(date) 
                    FROM dwarf_skills 
                    WHERE dwarf_id = ds.dwarf_id 
                        AND skill_id = ds.skill_id 
                        AND date < e.departure_date)                         
            GROUP BY ds.dwarf_id)                   AS de_before ON de_before.dwarf_id = de_after.dwarf_id                 
                                                    -- важноее условие, именно '= de_after.dwarf_id', 
                                                    -- мы берем только те навыки, прогресс по которым нашли.
    WHERE
        -- По логике успешность экспедиции можем определить 
        -- только если она завершилась.
        e.status = 'Completed'   
    GROUP BY e.expedition_id, e.destination, e.status
    ) AS agg
    


-- =========================================================================
-- (ДЛЯ ИСТОРИИ). Первая неудачная попытка, перегружено, тяжело читаемо,
-- проблемы с вычислением skill_improvement и overall_success_score.



SELECT
    e.expedition_id             AS expedition_id,
    e.destination               AS destination,
    e.status                    AS status,
    agg_em.survival_rate,           AS survival_rate,
    ea.artifacts_value          AS artifacts_value,
    es.discovered_sites         AS discovered_sites,
    ec.encounter_success_rate   AS encounter_success_rate,
    (
        SELECT *
        FROM dwarf_skills
        
    ) AS skill_improvement,
    DATEDIFF(e.return_date - e.departure_date) as expedition_duration,
    
    JSON_OBJECT(
        'member_ids',   agg_em.member_ids
        'artifact_ids', ea.artifact_ids,
        'site_ids',     es.site_ids
    ) as related_entities
FROM expeditions e
SELECT
FROM EXPEDITIONS
LEFT OUTER JOIN expedition_members
LEFT OUTER JOIN DWARF_SKILLS dsMax 
LEFT OUTER JOIN (
        SELECT 
            m.expedition_id,
            JSON_ARRAYAGG(m.dwarf_id) AS member_ids,
            COUNT(CASE WHEN m.survived THEN 1 END) / COUNT(*) * 100 AS survival_rate,
            SUM(si.skill_improvement) AS skill_improvement
        FROM expedition_members m
        LEFT OUTER JOIN dwarf_skills AS sk_im ON sk_im 
        
        LEFT OUTER JOIN (
                SELECT
                    expedition_id,
                    skill_id,
                    experience
                FROM dwarf_skills ds1
                WHERE date = (SELECT MAX(date) FROM dwarf_skills ds2 WHERE ds2.dwarf_id = ds1.dwarf_id AND date <= e.return_date)
                ORDER BY DESC date) as maxex
            AS si 
            ON si.dwarf_id = m.dwarf_id
        GROUP BY expedition_id)
    AS agg_em 
    ON agg_em.expedition_id = e.expedition_id
LEFT OUTER JOIN (
        SELECT 
            m.expedition_id,
            JSON_ARRAYAGG(m.dwarf_id) AS member_ids,
            COUNT(CASE WHEN m.survived THEN 1 END) / COUNT(*) * 100 AS survival_rate,
            SUM(si.skill_improvement) AS skill_improvement
        FROM expedition_members m
        LEFT OUTER JOIN (
                SELECT
                    dwarf_id,
                    skill_id,
                    experience
                FROM dwarf_skills ds1
                WHERE date = (SELECT MAX(date) FROM dwarf_skills ds2 WHERE ds2.dwarf_id = ds1.dwarf_id AND date <= e.return_date)
                ORDER BY DESC date) 
            AS si 
            ON si.dwarf_id = m.dwarf_id
        GROUP BY expedition_id)
    AS agg_em 
    ON agg_em.expedition_id = e.expedition_id
LEFT OUTER JOIN (
        SELECT
            expedition_id,
            JSON_ARRAYAGG(artifact_id) AS artifact_ids,
            SUM(value) AS artifacts_value
        FROM expedition_artifacts
        GROUP BY expedition_id)
    AS ea 
    ON ea.expedition_id = e.expedition_id
LEFT OUTER JOIN (
        SELECT 
            expedition_id,
            JSON_ARRAYAGG(site_id) AS site_ids,
            COUNT(*) AS discovered_sites
        FROM expedition_sites
        GROUP BY expedition_id)
    AS es 
    ON es.expedition_id = e.expedition_id
LEFT OUTER JOIN (
        SELECT
            expedition_id,
            COUNT(CASE WHEN outcome = 'Success' THEN 1 END) / COUNT(*) * 100 AS encounter_success_rate
        FROM expedition_creatures
        GROUP BY expedition_id)
    AS ec
    ON ec.expedition_id = e.expedition_id
LEFT OUTER JOIN (
        SELECT
            ds.dwarf_id,
            
        FROM dwarf_skills ds
        WHERE date <= () AND date <=
        GROUP BY dwarf_id = m.dwarf_id) 
    AS si
    ON si.dwarf_id = m.dwarf_id

WHERE
    -- По логике успешность экспедиции можем определить 
    -- только если она завершилась.
    e.status = 'Completed'