namespace AlgorithmsDataStructures2
{
    // ========= Урок 6  ===========

    // Задание 1.
    //  В процессе построения дерева проставить глубину каждому узлу.
    // Сложность временная: O(N), где N размер массива на входе.
    // Сложность по памяти: O(N), где N размер массива на входе.
    // (код в основном файле)


    // Задание 2.
    // Метод проверки, действительно ли дерево получилось правильным
    // (левый потомок меньше родителя, правый больше или равен родителю).
    // Сложность временная: O(N), где N кол-во узлов дерева.
    // Сложность по памяти: O(N), где N кол-во узлов дерева.
    public partial class BalancedBST
    {
        public bool IsSearchTree(BSTNode root_node)
        {
            if (root_node == null)
                return true;

            return IsSearchTree(root_node.LeftChild, int.MinValue, root_node.NodeKey, false)
                   && IsSearchTree(root_node.RightChild, root_node.NodeKey, int.MaxValue, true);
        }

        private bool IsSearchTree(BSTNode root_node, int min, int max, bool isRight)
        {
            if (root_node == null)
                return true;

            if (root_node.NodeKey < min)
                return false;

            if (!isRight && root_node.NodeKey == min)
                return false;

            if (root_node.NodeKey >= max)
                return false;

            return IsSearchTree(root_node.LeftChild, min, root_node.NodeKey, false)
                   && IsSearchTree(root_node.RightChild, root_node.NodeKey, max, true);
        }
    }

    // Задание 3.
    // Метод проверки, что дерево сбалансировано.
    // Сложность временная: O(N), где N кол-во узлов дерева.
    // Сложность по памяти: O(N), где N кол-во узлов дерева.
    // (код в основном файле)


    // ========================================================================

    // Рефлексия по уроку 4.

    // Оба задания выполнены корректно, придраться не к чему.
}
