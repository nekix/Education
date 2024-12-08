﻿Стандартный для C# словарь реализован классом Dictionary<TKey,  TValue>. Его реализация основана на использовании двух массивов buckets: int[] и entries: Entry[]. Структура Entry содержит в себе хеш-код (int), ключ (TKey), значение элемента (TValue) и индекс следующего элементе в массиве Entry (int). В чем основная идея:
<br/><br/>
* При операциях с элементами (ключ + значение) сначала вычисляется хеш-код ключа. Хеш-код ключа приводится в рамках длины массива buckets (hash % buckets.Length). Полученное значение является индексом элемента в массиве buckets. Массив buckets хранит номера индексов элементов массива entries.
* Массив entries является по факту массивом “однонаправленных связных списков”. Индексы, которые хранит массив buckets указывают на корневые элементы Entry этих связных списков в массиве entries, а сами элементы Entry хранят ссылки на следующие элементы, либо маркер конца списка.
* Получается, что каждый такой список хранит элементы, получившие в результате коллизии один и тот же индекс массива buckets.
<br/><br/>
- Поиск элемента заключается в вычислении из хеша индекса элемента в массиве buckets, указывающего на индекс первого элемента связного списка в массиве entries. После получения первого Entry элемента из массива entries происходит сравнение его и последующих в данном связном списке на совпадение хеш-кода и ключа элемента с искомым.
- Найденное совпадение и является искомым элементом Entry. В нем хранится поле value, которое возвращается как результат поиска.
- Поиск в худшем случае происходит за O(N), где N – число элементов (Entry). При малом числе коллизий сложность стремиться к O(1).
<br/><br/>
+ Добавление (перезапись) элемента в начале повторяет операцию поиска элемента, но вместо возврата значения происходит создание нового (перезапись value в имеющемся) Entry в массиве entries. При необходимости добавляется индекс на добавленный Entry в массив buckets и ссылка в добавляемом Entry не предыдущий первый элемент в связном списке.добавляемого элемента. Если все совпало, то значение value найденного Entry перезаписывается на новое (добавляемое). При необходимости, массивы расширяются.
+ Добавление происходит за O (K), где K – глубина коллизии (обычно приближается к O(1)). При расширении массива добавление становится O(N).
<br/><br/>
* Удаление реализовано сходным образом с добавлением. Сложность аналогичная. Необходимости в смещении элементов массивов нет, т.к. удаление реализуется по большей части за счёт манипулирования индексами в buckets и индексами на следующие элементы внутри связных списков Entry в entries.
<br/><br/>
- Словарь также расширяется при необходимости путем расширения массивов buckets и entries и корректировки значений (индексов на entries) в массиве buckets. Расширение происходиит за O(N).
<br/><br/>
+ Словарь реализует автоматическую операцию перехеширования при превышении глубины коллизий в 100. Перехеширование происходит за O(N).