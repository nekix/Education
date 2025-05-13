-- Задача 5*: Многофакторный анализ угроз и безопасности крепости

/* Возвращаемый REST

{
  "total_recorded_attacks": 183,
  "unique_attackers": 42,
  "overall_defense_success_rate": 76.50,
  "security_analysis": {
    "threat_assessment": {
      "current_threat_level": "Moderate",
      "active_threats": [
        {
          "creature_type": "Goblin",
          "threat_level": 3,
          "last_sighting_date": "0205-08-12",
          "territory_proximity": 1.2,
          "estimated_numbers": 35,
          "creature_ids": [124, 126, 128, 132, 136]
        },
        {
          "creature_type": "Forgotten Beast",
          "threat_level": 5,
          "last_sighting_date": "0205-07-28",
          "territory_proximity": 3.5,
          "estimated_numbers": 1,
          "creature_ids": [158]
        }
      ]
    },
    "vulnerability_analysis": [
      {
        "zone_id": 15,
        "zone_name": "Eastern Gate",
        "vulnerability_score": 0.68,
        "historical_breaches": 8,
        "fortification_level": 2,
        "military_response_time": 48,
        "defense_coverage": {
          "structure_ids": [182, 183, 184],
          "squad_ids": [401, 405]
        }
      }
    ],
    "defense_effectiveness": [
      {
        "defense_type": "Drawbridge",
        "effectiveness_rate": 95.12,
        "avg_enemy_casualties": 12.4,
        "structure_ids": [185, 186, 187, 188]
      },
      {
        "defense_type": "Trap Corridor",
        "effectiveness_rate": 88.75,
        "avg_enemy_casualties": 8.2,
        "structure_ids": [201, 202, 203, 204]
      }
    ],
    "military_readiness_assessment": [
      {
        "squad_id": 403,
        "squad_name": "Crossbow Legends",
        "readiness_score": 0.92,
        "active_members": 7,
        "avg_combat_skill": 8.6,
        "combat_effectiveness": 0.85,
        "response_coverage": [
          {
            "zone_id": 12,
            "response_time": 0
          },
          {
            "zone_id": 15,
            "response_time": 36
          }
        ]
      }
    ],
    "security_evolution": [
      {
        "year": 203,
        "defense_success_rate": 68.42,
        "total_attacks": 38,
        "casualties": 42,
        "year_over_year_improvement": 3.20
      },
      {
        "year": 204,
        "defense_success_rate": 72.50,
        "total_attacks": 40,
        "casualties": 36,
        "year_over_year_improvement": 4.08
      }
    ]
  }
}

*/

-- Запрос

WITH creatures_stats AS (
    SELECT
        c.type                                      AS creature_type,
        ROUND(AVG(c.threat_level), 2)               AS threat_level,
        MAX(cs.date)                                AS last_sighting_date
        ROUND(AVG(ct.distance_to_fortress), 2)      AS territory_proximity,
        SUM(c.estimated_population)                 AS estimated_numbers,
    FROM
        creatures c
    INNER JOIN
        creature_territories ct ON ct.creature_id = c.creature_id
    INNER JOIN
        creature_sightings cs ON cs.creature_id = c.creature_id
    GROUP BY
        c.type
), military_stats AS (
    SELECT
        ms.squad_id                                                                     AS squad_id,
        ms.name                                                                         AS squad_name,
        -- Принял, что 10 - максимальный уровень навыков
        COALESCE(ROUND(0.5 * AVG(ds.level) 
            / 10::DECIMAL +
        + 0.5 * 1::DECIMAL 
            / NULLIF((1 + 0.1 * AVG(ca.military_response_time_minutes)), 0), 2), 0)     AS readiness_score,
        COUNT(DISTINCT(sm.dwarf_id))                                                    AS active_members,
        AVG(ds.level)                                                                   AS avg_combat_skill,
        0.5::DECIMAL * (1 - COALESCE(ROUND(0.5::DECIMAL * AVG(ca.casualties))
            / NULLIF(AVG(ca.enemy_casualties), 1)
        + 0.5::DECIMAL * SUM(CASE WHEN ca.OUTCOME = 'Victory' THEN 1 END) 
            / NULLIF(COUNT(cf.OUTCOME), 0), 2), 0)                                      AS combat_effectiveness
    FROM
        military_squads ms
    LEFT OUTER JOIN
        squad_members sm ON sm.squad_id = ms.squad_id
    LEFT OUTER JOIN
        dwarf_skills ds ON ds.dwarf_id = sm.dwarf_id
        AND ds.date = (
            SELECT MAX(date)
            FROM dwarf_skills
            WHERE dwarf_id = sm.dwarf_id AND skill_id = ds.skill_id
        )
    LEFT OUTER JOIN
        skills s ON s.skill_id = ds.skill_id   
    -- у меня в схеме БД нет структуры этой таблицы, 
    -- только текстовое упоминание, поэтому предполагаю что так...
    LEFT OUTER JOIN 
        squad_battle_participation sbp ON sbp.squad_id = ms.squad_id
    LEFT OUTER JOIN
        creature_attacks ca ON ca.attack_id = sbp.attack_id
    WHERE
        sm.exit_date IS NULL AND s.category IN ('Combat', 'Military')
    GROUP BY
        ms.squad_id, ms.name
), locations_zones_stats AS (
    SELECT
        l.zone_id                                                   AS zone_id,
        l.name                                                      AS zone_name, -- или l.zone_type    
        1 - 
        (
            -- Предположил, что максимальный уровень 10
            0.25 * AVG(l.fortification_level) / 10::DECIMAL
            + 0.25 * AVG(l.wall_integrity) / 10::DECIMAL 
            + 0.25 * AVG(l.trap_density) / 10::DECIMAL 
            + 0.25 * GREATEST(1 - 
                0.1 * AVG(ca.military_response_time_minutes), 0)
        )                                                           AS vulnerability_score,
        SUM(CASE WHEN ca.outcome = 'Defeat' THEN 1 END)             AS historical_breaches,
        COALESCE(AVG(l.fortification_level), 2)                     AS fortification_level,
        COALESCE(AVG(ca.military_response_time_minutes), 2)         AS military_response_time
    FROM
        locations l
    LEFT OUTER JOIN
        creature_attacks ca ON ca.location_id = l.location_id    
    GROUP BY
        l.zone_id, l.name
), defense_types_stats AS (
    SELECT
        ds.defense_type                                                   AS defense_type,
        ROUND(100 * GREATEST(1::DOUBLE - AVG(ca.casualties) 
            / NULLIF(AVG(ca.enemy_casualties), 0::DOUBLE), 0), 2)         AS effectiveness_rate,
        ROUND(AVG(ca.enemy_casualties) 
            / NULLIF(AVG(JSON_LENGTH(ca.defense_structures_used)), 0), 2) AS avg_enemy_casualties
    -- у меня в схеме БД нет структуры этой таблицы, 
    -- только текстовое упоминание, поэтому предполагаю что так...
    FROM
        defense_structures ds
    LEFT OUTER JOIN
        creature_attacks ca ON JSON_CONTAINS(ca.defense_structures_used, ds.structure_id, '$')
    GROUP BY
        ds.defense_type
), year_security_stats AS (
    SELECT
        EXTRACT(YEAR FROM ca.date)                                      AS year,
        ROUND(100::DOUBLE * SUM(CASE ca.outcome = 'Victory' THEN 1 END) 
            / NULLIF(COUNT(DISTINCT(ca.attack_id)), 0), 2)              AS defense_success_rate,
        COUNT(ca.attack_id)                                             AS total_attacks,
        SUM(ca.casualties)                                              AS casualties,
        ROUND(SUM(ca.casualties)::DOUBLE 
            / NULLIF(SUM(ca.enemy_casualties), 0), 2)                   AS casualties_rate
    FROM
        creature_attacks ca
    GROUP BY
        EXTRACT(YEAR FROM ca.date)
)

SELECT
    (SELECT SUM(total_attacks) FROM year_security_stats)            AS total_recorded_attacks,
    (SELECT COUNT(DISTINCT(creature_id)) FROM creature_attacks)     AS unique_attackers,
    (SELECT SUM(defense_success_rate) FROM year_security_stats)     AS overall_defense_success_rate,
    JSON_OBJECT(
        'threat_assessment', JSON_OBJECT(
            'current_threat_level', (
                SELECT AVG(all_cs.threat_level)
                FROM creatures_stats all_cs
            ),
            'active_threats', (
                SELECT JSON_ARRAYAGG(
                    JSON_OBJECT(
                        'creature_type', cs.creature_type,
                        'threat_level', cs.threat_level,
                        'last_sighting_date', cs.last_sighting_date,
                        'territory_proximity', cs.territory_proximity,
                        'estimated_numbers', cs.estimated_numbers,
                        'creature_ids', (
                            SELECT JSON_ARRAYAGG(c.creature_id)
                            FROM creatures c
                            WHERE c.type = cs.creature_type
                        )
                    )
                ) FROM creatures_stats cs
            )
        ),
        'vulnerability_analysis', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'zone_id', lzs.zone_id,
                    'zone_name', lzs.zone_name,
                    'vulnerability_score', lzs.vulnerability_score,
                    'historical_breaches', lzs.historical_breaches,
                    'fortification_level', lzs.fortification_level,
                    'military_response_time', lzs.military_response_time,
                    'defense_coverage', JSON_OBJECT(
                        'structure_ids', (
                            SELECT JSON_ARRAYAGG(ds.structure_id)
                            -- у меня в схеме БД нет структуры этой таблицы, 
                            -- только текстовое упоминание, поэтому предполагаю что так...
                            FROM defense_structures ds
                            INNER JOIN locations l ON l.location_id = ds.location_id
                            WHERE l.zone_id = lzs.zone_id
                        ),
                        'squad_ids', (
                            SELECT JSON_ARRAYAGG(ms.squad_id)
                            -- у меня в схеме БД нет структуры этой таблицы, 
                            -- только текстовое упоминание, поэтому предполагаю что так...
                            FROM military_stations ms
                            INNER JOIN locations l ON l.location_id = ms.location_id
                            WHERE l.zone_id = lzs.zone_id
                        )
                    )
                )
            ) FROM locations_zones_stats lzs
        ),
        'defense_effectiveness', (
            SELECT JSON_ARRAYAGG(
                'defense_type', dts.defense_type,
                'effectiveness_rate', dts.effectiveness_rate,
                'avg_enemy_casualties', dts.avg_enemy_casualties
                'structure_ids', (
                    SELECT JSON_ARRAYAGG(ds.structure_id)
                    -- у меня в схеме БД нет структуры этой таблицы, 
                    -- только текстовое упоминание, поэтому предполагаю что так...
                    FROM defense_structures ds
                    WHERE ds.defense_type = dts.defense_type
                )
            ) FROM defense_types_stats dts
        ),
        'military_readiness_assessment', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'squad_id', ms.squad_id,
                    'squad_name', ms.squad_name,
                    'readiness_score', ms.readiness_score,
                    'active_members', ms.active_members,
                    'avg_combat_skill', ms.avg_combat_skill,
                    'combat_effectiveness', ms.combat_effectiveness,
                    'response_coverage', (
                        SELECT JSON_ARRAYAGG(
                            (
                                SELECT JSON_OBJECT(
                                    'zone_id', mcz.zone_id,
                                    'response_time', mcz.response_time
                                )
                                -- у меня в схеме БД нет структуры этой таблицы, 
                                -- только текстовое упоминание, поэтому предполагаю что так...
                                FROM military_coverage_zones mcz
                                WHERE mcz.squad_id = ms.squad_id
                            )
                        ) FROM (SELECT 1) AS also_dummy
                    )
                )
            ) FROM military_stats ms
        ),
        'security_evolution', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'year', yss.year,
                    'defense_success_rate', yss.defense_success_rate,
                    'total_attacks', yss.total_attacks,
                    'casualties', yss.casualties,
                    'year_over_year_improvement', (
                        COALESCE(yss.defense_success_rate - LAG(yss.defense_success_rate) OVER (ORDER BY yss.year) 
                        + yss.casualties_rate - LAG(yss.casualties_rate) OVER (ORDER BY yss.year), 0)
                    )
                )
            ) FROM year_security_stats yss
        )
    ) AS security_analysis,
FROM (SELECT 1) AS dummy;