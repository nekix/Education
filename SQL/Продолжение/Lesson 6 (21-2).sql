-- Задача 2. Получение данных о гноме с навыками и назначениями

/* Возвращаемый REST
[
  {
    "dwarf_id": 101,
    "name": "Urist McMiner",
    "age": 65,
    "profession": "Miner",
    "related_entities": {
      "skill_ids": [1001, 1002, 1003],
      "assignment_ids": [2001, 2002],
      "squad_ids": [401],
      "equipment_ids": [5001, 5002, 5003]
    }
  }
]
*/

-- Запрос
SELECT
    d.dwarf_id,
	d.name,
	d.age,
	d.profession,
	JSON_OBJECT(
        'skill_ids', (
            SELECT JSON_ARRAYAGG(ds.skill_id)
			FROM dward_skills ds
			WHERE ds.dwarf_id = d.dwarf_id
		),
		'assignment_ids', (
            SELECT JSON_ARRAYAGG(da.assignment_id)
			FROM dwarf_assignments da
			WHERE da.dwarf_id = d.dwarf_id
			    AND end_date IS NULL
		),
		'squad_ids', (
		    SELECT JSON_ARRAYAGG(sm.squad_id)
			FROM squad_members sm
            WHERE sm.dwarf_id = d.dwarf_id
			    AND exit_date IS NULL
		)
		'equipment_ids', (
		    SELECT JSON_ARRAYAGG(de.equipment_id)
			FROM dwarf_equipment de
			WHERE de.dwarf_id = d.dwarf_id
			-- Возможны дубликаты, одна и таже снаряга разного качества
			GROUP BY de.equipment_id			
		)
	) AS related_entities
FROM
    dwarves d;


-- Задача 3. Данные о мастерской с назначенными рабочими и проектами

/* Возвращаемый REST
[
  {
    "workshop_id": 301,
    "name": "Royal Forge",
    "type": "Smithy",
    "quality": "Masterwork",
    "related_entities": {
      "craftsdwarf_ids": [101, 103],
      "project_ids": [701, 702, 703],
      "input_material_ids": [201, 204],
      "output_product_ids": [801, 802]
    }
  }
]
*/

SELECT
    w.workshop_id,
	w.name,
	w.type,
	w.quality,
	JSON_OBJECT(
	    'craftsdwarf_ids', (
		    SELECT JSON_ARRAYAGG(cd.dwarf_id)
		    FROM workshop_craftsdwarves cd
			WHERE cd.workshop_id = w.workshop_id
		)
		'project_ids', (
		    SELECT JSON_ARRAYAGG(proj.project_id)
			FROM projects proj
			WHERE proj.workshop_id = w.workshop_id
			-- Если статус показывает, текущий это проект или нет,
			-- то, это нужно учитывать, что-то вроде:
			AND status != 'Сompleted'
		)
		'input_аmaterial_ids', (
		    SELECT JSON_ARRAYAGG(m.material_id)
			FROM workshop_materials m
			WHERE m.workshop_id = w.workshop_id
			-- Учитывая, что в описании workshop_materials указано,
			-- что это 'используемые материалы', то не слишком понятно,
			-- а что значит тогда is_input. Предположил что всё-таки это флажок
			-- именно для используемых материалов, если нет, то следующий AND убрать.
			    AND m.is_input = TRUE
            -- А вообще, я не могу понять,
			-- где табличка 'materials' или что-то вроде, на что FK)				
		)
		'output_product_ids', (
		    SELECT JSON_ARRAYAGG(product_id)
			-- Вообще непонятно, у нас workshop_products n:m
			-- и при этом products к workshop 1:n, где-то нарушена
			-- логика (а может продукты сами могут выступать материалами...).
			-- Придётся выгружать и оттуда и оттуда, хотя выглядит это очень плохо
			-- и по хорошему нужно покопаться в определениях таблиц в такой базе, 
			-- чтобы понять, что на самом деле происходит
			FROM(
			    SELECT wprod.product_id
				FROM workshop_products wprod
			    WHERE wprod.workshop_id = w.workshop_id
			    UNION
			    SELECT prod.product_id
			    FROM products prod
			    WHERE prod.workshop_id = w.workshop_id
			) AS product_id				
			-- Я вот честно не знаю, как поступит UNION с дубликатами внутри каждой из
            -- двух выборок, знаю наверняка что удаляет дубликаты, если запись есть и в
			-- в первой и во второй выборке. А тут дубликаты могут быть внутри именно первой
			-- выборки точно. Мне кажется удаляет и их. Если не удаляет, то нужна ещё и группировка.
			-- GROUP BY product_id
		)
	) AS related_entities
FROM
    workshops w;
	
-- Задача 4. Данные о военном отряде с составом и операциями

/* Возвращаемый REST

[
  {
    "squad_id": 401,
    "name": "The Axe Lords",
    "formation_type": "Melee",
    "leader_id": 102,
    "related_entities": {
      "member_ids": [102, 104, 105, 107, 110],
      "equipment_ids": [5004, 5005, 5006, 5007, 5008],
      "operation_ids": [601, 602],
      "training_schedule_ids": [901, 902],
      "battle_report_ids": [1101, 1102, 1103]
    }
  }
]

*/

SELECT
    ms.squad_id,
	ms.name,
	ms.formation_type,
	ms.leader_id,
	JSON_OBJECT(
	    'member_ids', (
		    SELECT JSON_ARRAYAGG(sm.dwarf_id)
			FROM squad_members sm
			WHERE sm.squad_id = ms.squad_id
			    AND exit_date IS NULL
		),
		'equipment_ids', (
		    SELECT JSON_ARRAYAGG(se.equipment_id)
			FROM squad_equipment se
			WHERE se.squad_id = ms.squad_id
		),
		'operation_ids', (
		    SELECT JSON_ARRAYAGG(so.operation_id)
			FROM squad_operations so
			WHERE so.squad_id = ms.squad_id
			-- возможно здесь также хранятся планируемые операции
			-- и их не нужно выгружать (если start_date > now), 
			-- в таком случае можно сделать доп. проверку:
			-- AND start_date <= NOW()
			-- ну или проверять status, надо опять таки конкретно смотреть что и как
		),
		'training_schedule_ids', (
		    SELECT JSON_ARRAYAGG(st.schedule_id)
			FROM squad_training st
			WHERE st.squad_id = ms.squad_id
			-- тоже самое, что и для squad_operations, только для date
		),
		'battle_report_ids', (
		    SELECT JSON_ARRAYAGG(sb.report_id)
			FROM squad_battles sb
			WHERE sb.squad_id = ms.squad_id
		)
	) as related_entities
FROM
    military_squads ms;