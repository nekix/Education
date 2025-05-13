/*

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

-- пока не понимаю как привязать к замку...
WITH creatures_stats AS (
    SELECT
        c.type                          AS creature_type,
        c.threat_level                  AS threat_level,
        MAX(cs.date)                    AS last_sighting_date
        ct.distance_to_fortress         AS territory_proximity,
        c.estimated_population          AS estimated_numbers,
        JSON_ARRAYAGG(c.creature_id)    AS creature_ids
    FROM
        creatures c
    INNER JOIN
        creature_territories ct ON ct.creature_id = c.creature_id
    INNER JOIN
        creature_sightings cs ON cs.creature_id = c.creature_id
    GROUP BY
        c.type, c.threat_level
), military_stats AS (
    SELECT
        ms.squad_id                                                                     AS squad_id,
        ms.name                                                                         AS squad_name,
        -- Принял, что 10 - максимальный уровень навыков
        COALESCE(ROUND(0.5 * AVG(ds.level) / 10::DECIMAL +
        + 0.5 * 1::DECIMAL 
            / NULLIF((1 + 0.1 * AVG(ca.military_response_time_minutes)), 0), 2), 0)     AS readiness_score,
        COUNT(DISTINCT(sm.dwarf_id))                                                    AS active_members,
        AVG(ds.level)                                                                   AS avg_combat_skill,
        COALESCE(ROUND(0.5::DECIMAL * AVG(ca.enemy_casualties) 
            / NULLIF(AVG(ca.casualties), 0)
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
    LEFT OUTER JOIN
        locations l ON l.location_id = ca.location_id
    WHERE
        sm.exit_date IS NULL AND s.category IN ('Combat', 'Military')
    GROUP BY
        ms.squad_id, ms.name
), locations_stats AS (
    SELECT
        l.zone_id                                   AS zone_id,
        l.name                                      AS zone_name, -- или l.zone_type
        AS vulnerability_score,
        SUM(CASE ca.outcome = 'Defeat' THEN 1 END)  AS historical_breaches,
        AVG(l.fortification_level)                  AS fortification_level,
        AVG(ca.military_response_time_minutes)      AS military_response_time
    FROM
        locations l
    LEFT OUTER JOIN
        creature_attacks ca ON ca.location_id = l.location_id    
    GROUP BY
        l.zone_id, l.name
)







SELECT
    AS total_recorded_attacks,
    AS unique_attackers,
    AS overall_defense_success_rate,
    JSON_OBJECT(
        'threat_assessment', JSON_OBJECT(
            'current_threat_level',
            'active_threats', (
                SELECT JSON_ARRAYAGG(
                    JSON_OBJECT(
                        'creature_type',
                        'threat_level',
                        'last_sighting_date',
                        'territory_proximity',
                        'estimated_numbers',
                        'creature_ids'
                    )
                ) FROM
            )
        ),
        'vulnerability_analysis', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'zone_id',
                    'zone_name',
                    'vulnerability_score',
                    'historical_breaches',
                    'fortification_level',
                    'military_response_time',
                    'defense_coverage', JSON_OBJECT(
                        'structure_ids',
                        'squad_ids'
                    )
                )
            ) FROM 
        ),
        'defense_effectiveness', (
            SELECT JSON_ARRAYAGG(
                'defense_type',
                'effectiveness_rate',
                'avg_enemy_casualties',
                'structure_ids'
            ) FROM
        ),
        'military_readiness_assessment', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'squad_id',
                    'squad_name',
                    'readiness_score',
                    'active_members',
                    'avg_combat_skill',
                    'combat_effectiveness',
                    'response_coverage', (
                        SELECT JSON_ARRAYAGG(
                            JSON_OBJECT(
                                'zone_id', l.zone_id
                                'response_time', ca.military_response_time_minutes
                            )
                        )
                        -- у меня в схеме БД нет структуры этой таблицы, 
                        -- только текстовое упоминание, поэтому предполагаю что так...
                        FROM squad_battle_participation sbp
                        LEFT OUTER JOIN  creature_attacks ca ON ca.attack_id = sbp.attack_id
                        LEFT OUTER JOIN locations l ON l.location_id = ca.location_id
                        --WHERE sbp.squad_id = ms.squad_id 
                        GROUP BY l.zone_id
                    )
                )
            ) FROM
        ),
        'security_evolution', (
            SELECT JSON_ARRAYAGG(
                JSON_OBJECT(
                    'year',
                    'defense_success_rate',
                    'total_attacks',
                    'casualties',
                    'year_over_year_improvement'
                )
            ) FROM
        )
    ) AS security_analysis,


FROM