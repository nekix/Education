/*
Exercise 1
*/

SELECT
    d.dwarf_id,
    d.name,
    d.age,
    d.profession,
    d.squad_id,
    s.name as squad_name,
    s.mission as squad_mission
FROM Dwarves d
	JOIN Squads s ON s.squad_id = d.squad_id

/*
Exercise 2
*/

SELECT *
FROM Dwarves d
WHERE d.profession = 'miner' AND d.squad_id IS NULL

/*
Exercise 3
*/

SELECT *
FROM Tasks
WHERE status = 'pending' AND priority = (
    SELECT MAX(t.priority) FROM Tasks t WHERE t.status = 'pending');

/*
Exercise 4
*/

SELECT
    d.dwarf_id,
    d.name,
    d.age,
    d.profession,
    d.squad_id,
    COUNT(d) as items_count
FROM Dwarves d
JOIN Items i ON d.dwarf_id = i.owner_id
GROUP BY d.dwarf_id;

/*
Exercise 5
*/

SELECT
    s.squad_id,
    s.name,
    s.mission,
    count(d) as dwarfs_count
FROM Squads s
LEFT JOIN Dwarves d ON s.squad_id = d.squad_id
GROUP BY s.squad_id

/*
Exercise 6
*/

SELECT
    d.profession,
    COUNT(t) as uncompleted_tasks_count
FROM Dwarves d
JOIN Tasks t on d.dwarf_id = t.assigned_to and t.status != 'completed'
GROUP BY d.profession
HAVING COUNT(*) = (
    SELECT MAX(uncompletedTasks.count)
    FROM (
        SELECT COUNT(*) as count
        FROM Tasks t2
        JOIN Dwarves d2 on d2.dwarf_id = t2.assigned_to and t2.status != 'completed' GROUP BY t2.assigned_to) as uncompletedTasks)

/*
Exercise 7
*/

SELECT
    i.type,
    AVG(d.age)
FROM Items i
LEFT JOIN Dwarves d on d.dwarf_id = i.owner_id
GROUP BY i.type

/*
Exercise 8
*/

SELECT d.*
FROM Dwarves d
LEFT JOIN Items i ON i.owner_id = d.dwarf_id
WHERE i.owner_id is null and d.age > (SELECT AVG(age) FROM Dwarves)