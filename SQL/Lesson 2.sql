/*
Exercise 1
*/

SELECT
    name AS SquadName
FROM
    Squads
WHERE
    leader_id IS NULL

/*
Exercise 2
*/

SELECT
    name AS DwarfName,
    age AS Age
FROM
    Dwarves
WHERE
    age > 150 AND profession = 'Warrior'

/*
Exercise 3
*/

SELECT
    d.name AS DwarfName,
    d.age AS Age,
    d.profession AS Profession
FROM
    Dwarves d
JOIN
    Items i
ON
    i.type = 'weapon' AND d.dward_id = i.owner_id
GROUP BY
    d.dward_id
    
/*
Exercise 4
*/

SELECT
    d.name AS DwarfName,
    d.age AS Age,
    d.profession AS Profession,
    t.status as TasksStatus,
    COUNT(t) as TasksCount
FROM
    Dwarves d
JOIN
    Tasks t
ON
    d.dwarf_id = t.assigned_to
GROUP BY
    d.dward_id, t.status
    
/*
Exercise 5
*/

SELECT
    t.task_id AS TaskId,
    t.description AS Description,
    t.status AS Status
FROM
    Tasks t
JOIN
    Dwarves d
ON
    t.assigned_to = d.dwarf_id
JOIN
    Squads s
ON
    d.squad_id = s.squad_id
WHERE
    s.name = 'Guardians'
    
    
/*
Exercise 6
*/

SELECT
    d1.name AS DwarfName,
    d2.name AS NearRelativeDwarf,
    d1.relationship AS Relationship
FROM
    Dwarves d1
JOIN
    Dwarves d2
ON
    d1.rerelated_to = d2.dwarf_id
WHERE
    d1.relationship IN ('Родитель', 'Ребенок', 'Супруг', 'Супруга', 'Брат', 'Сестра', 'Бабушка', 'Дедушка', 'Внук', 'Внучка')