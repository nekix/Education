using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using AlgorithmsDataStructures2;
using Shouldly;
using Xunit;

namespace Education.Ads.Tests.Exercise_5
{
    public class BalancedBST_Tests
    {
        [Theory]
        [MemberData(nameof(GetGenerateBBSTArrayData))]
        public void Should_GenerateBBSTArray(int[] a, int[] res)
        {
            BalancedBST.GenerateBBSTArray(a).ShouldBe(res);
        }

        public static IEnumerable<object[]> GetGenerateBBSTArrayData()
        {
            // 1: Пустой массив
            int[] a = new int[] {  };
            int[] res = new int[] {  };
            yield return new object[] { a, res };

            // 2: Один элемент
            a = new int[] { 7 };
            res = new int[] { 7 };
            yield return new object[] { a, res };

            // 3: Древо глубины 1
            a = new int[] { 12, 7, 1 };
            res = new int[] { 7, 1, 12 };
            yield return new object[] { a, res };

            // 5: Древо глубины 3 и полностью заполненное
            a = new int[] { 7, 1, 12, 5, 10, 3, 14, 8, 0, 6, 13, 2, 4, 9, 11 };
            res = new int[] { 7, 3, 11, 1, 5, 9, 13, 0, 2, 4, 6, 8, 10, 12, 14 };
            yield return new object[] { a, res };

            // 6: Уже отсортированный
            a = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            res = new int[] { 7, 3, 11, 1, 5, 9, 13, 0, 2, 4, 6, 8, 10, 12, 14 };
            yield return new object[] { a, res };

            // 7: Дерево из массива картинки предыдущего урока
            a = new int[] { 50, 25, 75, 10, 30, 60, 90, 5, 15, 27, 35, 55, 65, 85, 95 };
            res = new int[] { 50, 25, 75, 10, 30, 60, 90, 5, 15, 27, 35, 55, 65, 85, 95 };
            yield return new object[] { a, res };
        }
    }
}
