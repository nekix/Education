-- Задача 3*: Комплексная оценка военной эффективности отрядов

/* Возвращаемый REST

[
  {
    "squad_id": 401,
    "squad_name": "The Axe Lords",
    "formation_type": "Melee",
    "leader_name": "Urist McAxelord",
    "total_battles": 28,
    "victories": 22,
    "victory_percentage": 78.57,
    "casualty_rate": 24.32,
    "casualty_exchange_ratio": 3.75,
    "current_members": 8,
    "total_members_ever": 12,
    "retention_rate": 66.67,
    "avg_equipment_quality": 4.28,
    "total_training_sessions": 156,
    "avg_training_effectiveness": 0.82,
    "training_battle_correlation": 0.76,
    "avg_combat_skill_improvement": 3.85,
    "overall_effectiveness_score": 0.815,
    "related_entities": {
      "member_ids": [102, 104, 105, 107, 110, 115, 118, 122],
      "equipment_ids": [5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008, 5009],
      "battle_report_ids": [1101, 1102, 1103, 1104, 1105, 1106, 1107, 1108],
      "training_ids": [901, 902, 903, 904, 905, 906]
    }
  },
  {
    "squad_id": 403,
    "squad_name": "Crossbow Legends",
    "formation_type": "Ranged",
    "leader_name": "Dokath Targetmaster",
    "total_battles": 22,
    "victories": 18,
    "victory_percentage": 81.82,
    "casualty_rate": 16.67,
    "casualty_exchange_ratio": 5.20,
    "current_members": 7,
    "total_members_ever": 10,
    "retention_rate": 70.00,
    "avg_equipment_quality": 4.45,
    "total_training_sessions": 132,
    "avg_training_effectiveness": 0.88,
    "training_battle_correlation": 0.82,
    "avg_combat_skill_improvement": 4.12,
    "overall_effectiveness_score": 0.848,
    "related_entities": {
      "member_ids": [106, 109, 114, 116, 119, 123, 125],
      "equipment_ids": [5020, 5021, 5022, 5023, 5024, 5025, 5026],
      "battle_report_ids": [1120, 1121, 1122, 1123, 1124, 1125],
      "training_ids": [920, 921, 922, 923, 924]
    }
  }
]

*/

-- Запрос

WITH squad_info AS (
    SELECT
        ms.squad_id,
        ms.squad_name,
        ms.formation_type,
        d.name AS leader_name
    FROM
        military_squads ms
    INNER JOIN 
        dwarves d ON d.dwarf_id = ms.leader_id
),
battles_info AS (
    SELECT
        sb.squad_id,
        sb.date,
        COUNT(sb.squad_id) AS total_battles,
        COUNT(wsb.squad_id) AS victories,
        SUM(sb.casualties) AS casualties,
        SUM(sb.enemy_casualties) AS enemy_casualties
    FROM
        squad_battles sb
    LEFT OUTER JOIN
        squad_battles wsb ON wsb.squad_id = sb.squad_id AND swb.outcome = "Victory"
    GROUP BY
        sb.squad_id, sb.date    
),
members_info AS (
    SELECT
        tsm.squad_id,
        COUNT(DISTINCT(csm.dwarf_id)) AS current_members,
        COUNT(tsm.dwarf_id) AS total_members_ever
    FROM
        squad_members AS tsm
    LEFT OUTER JOIN
        squad_members AS csm ON csm.squad_id = tsm.squad_id 
            AND csm.exit_date IS NULL
    GROUP BY
        tsm.squad_id
),
equipment_info AS (
    SELECT
        se.squad_id,
        SUM(se.quantity * e.quality) / NULLIF(SUM(se.quantity), 0) AS avg_equipment_quality,
    FROM
        squad_equipment se
    INNER JOIN
        equipment e ON e.equipment_id = se.equipment_id
    GROUP BY
        se.squad_id
),
training_skills_info AS (
    SELECT
        st.squad_id,
        sm.dwarf_id, 
        ds.skill_id, 
        st.schedule_id,
        st.date,
        st.effectiveness,
        st.duration_hours,
        MAX(COALESCE(ds_after.level, 0)) - MIN(COALESCE(ds_before.level), 0) AS combat_skill_improvement
    FROM
        squad_training st       
    LEFT OUTER JOIN
        squad_members sm ON sm.squad_id = st.squad_id
    LEFT OUTER JOIN
        dwarf_skills ds_before ON ds_before.dwarf_id = sm.dwarf_id
    LEFT OUTER JOIN
        skills s ON s.skill_id = ds_before.skill_id AND s.category = 'Combat'
    LEFT OUTER JOIN
        dwarf_skills ds_after ON ds_after.dwarf_id = ds_before.dwarf_id
        AND ds_after.skill_id = ds_before.skill_id
    WHERE
        ds_before.date < st.date 
        AND ds_after.date > DATE_ADD(st.date, INTERVAL st.duration_hours HOUR)
    GROUP BY
        st.squad_id, sm.dwarf_id, ds.skill_id, st.schedule_id, st.date, st.effectiveness, st.duration_hours
),
training_battle_by_date AS (
    SELECT
        bi.squad_id,
        bi.enemy_casualties / bi.casualties + bi.victories / bi.total_battles AS battle_score,
        COUNT(tsi.squad_id) * AVG(tsi.effectiveness) * AVG(tsi.duration_hours) AS training_before_score
    FROM
        battles_info bi
    LEFT OUTER JOIN
        training_skills_info tsi ON tsi.squad_id = bi.squad_id 
        AND tsi.date < bi.date
    GROUP BY
        bi.squad_id, bi.date
)

SELECT
    si.squad_id,
    si.squad_name,
    si.formation_type,
    si.leader_name,
    
    COALESCE(SUM(bi.total_battles), 0)                                                      AS total_battles,
    COALESCE(SUM(bi.victories), 0)                                                          AS victories,
    COALESCE(ROUND(SUM(bi.victories) / NULLIF(SUM(bi.total_battles), 0) * 100, 2), 0)       AS victory_percentage,
    COALESCE(ROUND(SUM(bi.casualties) / NULLIF(mi.total_members_ever, 0) * 100, 2), 0)      AS casualty_rate,
    COALESCE(ROUND(SUM(bi.enemy_casualties) / NULLIF(SUM(bi.casualties), 0), 2), 0)         AS casualty_exchange_ratio,
    
    COALESCE(mi.current_members, 0)                                                         AS current_members,
    COALESCE(mi.total_members_ever, 0)                                                      AS total_members_ever,
    COALESCE(ROUND(mi.current_members / NULLIF(mi.total_members_ever, 0) * 100, 2), 0)      AS retention_rate,
    
    COALESCE(ROUND(ei.avg_equipment_quality, 2), 0)                                         AS avg_equipment_quality,
    COALESCE(COUNT(tsi.squad_id), 0)                                                        AS total_training_sessions,
    COALESCE(ROUND(AVG(tsi.effectiveness), 2), 0)                                           AS avg_training_effectiveness,
    
    CORR(tbdd.training_before_score, tbbd.battle_score)                                     AS training_battle_correlation,
    COALESCE(ROUND(AVG(tsi.combat_skill_improvement), 2), 0)                                AS avg_combat_skill_improvement,
    
    COALESCE(ROUND(SUM(bi.victories) / NULLIF(SUM(bi.total_battles), 0), 2), 0) +
        COALESCE(ROUND(ei.avg_equipment_quality / 10, 2), 0) +
        COALESCE(ROUND(mi.current_members / NULLIF(mi.total_members_ever, 0), 2), 0) +
        COALESCE(ROUND(AVG(tsi.effectiveness) / 100, 2), 0) +
        CORR(tbdd.training_before_score, tbbd.battle_score)                                 AS overall_effectiveness_score,
        
    JSON_OBJECT(
        'member_ids', (
            SELECT JSON_ARRAYAGG(ms.dwarf_id)
            FROM military_squads ms
            WHERE ms.squad_id = si.squad_id
        ),
        'equipment_ids', (
            SELECT JSON_ARRAYAGG(se.equipment_id)
            FROM squad_equipment se
            WHERE se.squad_id = si.squad_id
        ),
        'battle_report_ids', (
            SELECT JSON_ARRAYAGG(sb.report_id)
            FROM squad_battles sb
            WHERE sb.squad_id = si.squad_id
        ),
        'training_ids', (
            SELECT JSON_ARRAYAGG(st.schedule_id)
            FROM squad_training st
            WHERE st.squad_id = si.squad_id
        )
    ) AS related_entities
FROM
    squad_info si
LEFT OUTER JOIN
    battles_info bi ON bi.squad_id = si.squad_id
LEFT OUTER JOIN
    members_info mi ON mi.squad_id = si.squad_id
LEFT OUTER JOIN
    equipment_info ei ON ei.squad_id = si.squad_id
LEFT OUTER JOIN
    training_skills_info tsi ON tsi.squad_id = si.squad_id
LEFT OUTER JOIN
    training_battle_by_date tbbd ON tbbd.squad_id = si.squad_id
GROUP BY
    si.squad_id, si.squad_name, si.formation_type, si.leader_name,
    mi.current_members, mi.total_members_ever, ei.avg_equipment_quality