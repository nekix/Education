extern alias Exercise8;
using Exercise8.AlgorithmsDataStructures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ads.Tests.Exercise_8
{
    public class SoltHashTable_PutTests
    {
        [Theory]
        [MemberData(nameof(DdosData))]
        public void Should_Put_With_Many_Collisions(List<string> ddosData)
        {
            var hashTable = GetEmptyHashTable(220, 3);

            hashTable.Put(ddosData[0]);

            foreach (var item in ddosData.Skip(1))
                hashTable.Put(item);

            hashTable.MaxCollisionDeep.ShouldBeLessThan(3);
        }

        public static IEnumerable<Object[]> DdosData()
        {
            var ddosData = new List<string>();

            for (int i = 0; i < 17; i++)
            {
                var str = ((char)(63 - i)).ToString()
                    + ((char)(63 + i)).ToString();

                ddosData.Add(str);
            }

            yield return new object[] { ddosData };
        }

        private static SaltHashTable GetEmptyHashTable(int sz, int stp)
            => new SaltHashTable(sz, stp);
    }
}
