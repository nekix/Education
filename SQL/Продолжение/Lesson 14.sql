-- Задача 4*: Комплексный анализ торговых отношений и их влияния на крепость

/* Возвращаемый REST

{
  "total_trading_partners": 5,
  "all_time_trade_value": 15850000,
  "all_time_trade_balance": 1250000,
  "civilization_data": {
    "civilization_trade_data": [
      {
        "civilization_type": "Human",
        "total_caravans": 42,
        "total_trade_value": 5240000,
        "trade_balance": 840000,
        "trade_relationship": "Favorable",
        "diplomatic_correlation": 0.78,
        "caravan_ids": [1301, 1305, 1308, 1312, 1315]
      },
      {
        "civilization_type": "Elven",
        "total_caravans": 38,
        "total_trade_value": 4620000,
        "trade_balance": -280000,
        "trade_relationship": "Unfavorable",
        "diplomatic_correlation": 0.42,
        "caravan_ids": [1302, 1306, 1309, 1316, 1322]
      }
    ]
  },
  "critical_import_dependencies": {
    "resource_dependency": [
      {
        "material_type": "Exotic Metals",
        "dependency_score": 2850.5,
        "total_imported": 5230,
        "import_diversity": 4,
        "resource_ids": [202, 208, 215]
      },
      {
        "material_type": "Lumber",
        "dependency_score": 1720.3,
        "total_imported": 12450,
        "import_diversity": 3,
        "resource_ids": [203, 209, 216]
      }
    ]
  },
  "export_effectiveness": {
    "export_effectiveness": [
      {
        "workshop_type": "Smithy",
        "product_type": "Weapons",
        "export_ratio": 78.5,
        "avg_markup": 1.85,
        "workshop_ids": [301, 305, 310]
      },
      {
        "workshop_type": "Jewelery",
        "product_type": "Ornaments",
        "export_ratio": 92.3,
        "avg_markup": 2.15,
        "workshop_ids": [304, 309, 315]
      }
    ]
  },
  "trade_timeline": {
    "trade_growth": [
      {
        "year": 205,
        "quarter": 1,
        "quarterly_value": 380000,
        "quarterly_balance": 20000,
        "trade_diversity": 3
      },
      {
        "year": 205,
        "quarter": 2,
        "quarterly_value": 420000,
        "quarterly_balance": 35000,
        "trade_diversity": 4
      }
    ]
  }
}

*/

-- Запрос

WITH civilization_trade_data AS (
    SELECT
        c.civilization_type AS civilization_type,
        COUNT(DISTINCT(c.caravan_id)) AS total_caravans,
        COALESCE(ROUND(SUM(tt.value), 2), 0) AS total_trade_value,
        COALESCE(ROUND(SUM(CASE WHEN tt.balance_direction = 'Export' THEN tt.value ELSE -tt.value END), 2), 0) AS trade_balance,
        CASE
            WHEN COALESCE((SUM(CASE WHEN tt.balance_direction = 'Export' THEN tt.value ELSE -tt.value END)), 0) >= 0
            THEN 'Favorable'
            ELSE 'Unfavorable'
        END AS trade_relationship,
        COALESCE(
            ROUND(
                CORR(
                    CASE
                        WHEN de.outcome = 'Positive' THEN de.relationship_change
                        WHEN de.outcome = 'Negative' THEN -de.relationship_change
                        ELSE 0
                    END,
                    tt.value::DECIMAL
                ),
            2),
        0) AS diplomatic_correlation,
        JSON_ARRAYAGG(c.caravan_id) AS caravan_ids
    FROM
        caravans c
    LEFT OUTER JOIN
        trade_transactions tt ON tt.caravan_id = c.caravan_id
    LEFT OUTER JOIN
        diplomatic_events de ON de.caravan_id = c.caravan_id AND de.date <= tt.date
    GROUP BY
        c.civilization_type
), trade_timeline AS (
    SELECT
        YEAR(tt.date) AS year,
        QUARTER(tt.date) AS quarter,
        COALESCE(ROUND(SUM(tt.value), 2), 0) AS querterly_value,
        COALESCE(ROUND(SUM(CASE WHEN tt.balance_direction = 'Export' THEN tt.value ELSE -tt.value END), 2), 0) AS quarterly_balance,
        COUNT(DISTINCT(c.civilization_type)) AS trade_diversity
    FROM
        trade_transactions tt
    INNER JOIN
        caravans c ON c.caravan_id = tt.caravan_id
    GROUP BY
        YEAR(tt.date), QUARTER(tt.date)
), resource_dependency AS (
    SELECT
        cg.material_type,
        COALESCE(
            ROUND(
                SUM(cg.quantity) * AVG(cg.value::DECIMAL) * AVG(r.rarity)
                - NULLIF((COUNT(c.civilization_type) + COUNT(DISTINCT(es.site_id)) + AVG(es.depth) + AVG(es.accessibility)), 0), 
            2), 
        0) AS dependency_score,
        COALESCE(SUM(cg.quantity), 0) AS total_imported,
        COUNT(c.civilization_type) AS import_diversity,
        JSON_ARRAYAGG(es.resource_id) AS resource_ids
    FROM
        caravan_goods cg
    INNER JOIN
        caravans c ON c.caravan_id = cg.caravan_id
    INNER JOIN
        trade_transactions tt ON tt.caravan_id = c.caravan_id
    LEFT OUTER JOIN
        resources r ON r.type = cg.material_type
    LEFT OUTER JOIN
        extraction_sites es ON es.resource_id = r.resource_id
    WHERE
        tt.balance_direction = 'Import' AND JSON_CONTAINS(tt.caravan_items, cg.name, '$')
    GROUP BY
        cg.material_type
), export_effectiveness AS (
    SELECT
        w.type AS workshop_type,
        p.type AS product_type,
        COALESCE(ROUND(SUM(cg.quantity::DECIMAL) / NULLIF(SUM(wp.quantity), 0), 2) * 100, 0) AS export_ratio,
        COALESCE(ROUND(AVG(p.value) - AVG(cg.value), 2), 0) AS avg_markup,
        JSON_ARRAYAGG(w.workshop_id) AS workshop_ids
    FROM  
        workshops w
    JOIN
        workshop_products wp ON wp.workshop_id = w.workshop_id
    JOIN
        products p ON p.product_id = wp.product_id
    JOIN
        caravan_goods cg ON cg.original_product_id = p.product_id
    JOIN
        trade_transactions tt ON tt.caravan_id = cg.caravan_id
    WHERE
        AND JSON_CONTAINS(tt.caravan_items, cg.name, '$')
    GROUP BY
        w.type, p.type
)

SELECT
    COALESCE(COUNT(DISTINCT(ctd.civilization_type)), 0) AS total_trading_partners,
    COALESCE(SUM(ctd.total_trade_value), 0) AS all_time_trade_value,
    COALESCE(SUM(ctd.trade_balance), 0) AS all_time_trade_balance,
    JSON_OBJECT(
        'civilization_trade_data', JSON_ARRAYAGG(
            JSON_OBJECT(
                'civilization_type', ctd.civilization_type,
                'total_caravans', ctd.total_caravans,
                'total_trade_value', ctd.total_trade_value,
                'trade_balance', ctd.trade_balance,
                'trade_relationship', ctd.trade_relationship,
                'diplomatic_correlation', ctd.diplomatic_correlation,
                'caravan_ids', ctd.caravan_ids
            )
        )
    ) AS civilization_data,
    JSON_OBJECT(
        'resource_dependency', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'material_type', rd.material_type,
                    'dependency_score', rd.dependency_score,
                    'total_imported', rd.total_imported,
                    'import_diversity', rd.import_diversity
                    'resource_ids', rd.resource_ids
                )
            )
            FROM resource_dependency rd
        )
    ) AS critical_import_dependencies,
    JSON_OBJECT(
        'export_effectiveness', JSON_ARRAYAGG(
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'workshop_type', ef.workshop_type,
                    'product_type', ef.product_type,
                    'export_ratio', ef.export_ratio,
                    'avg_markup', ef.avg_markup,
                    'workshop_ids' ef.workshop_ids
                )
            )
            FROM export_effectiveness ef
        )
    ) AS export_effectiveness,
    JSON_OBJECT(
        'trade_growth', JSON_ARRAYAGG(
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'year', tt.year,
                    'quarter', tt.quarter,
                    'quarterly_value', tt.quarterly_value,
                    'quarterly_balance', tt.quarterly_balance,
                    'trade_diversity', tt.trade_diversity
                )
            )
            FROM trade_timeline tt
        )
    ) AS trade_timeline
FROM
    civilization_trade_data ctd
