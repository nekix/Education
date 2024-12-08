using AlgorithmsDataStructures;

namespace Ads.Exercise10
{
    public static class PowerSetExtensions
    {
        public static PowerSet<T> IntersectionMany<T>(this PowerSet<T> firstSet, params PowerSet<T>[] sets)
        {
            PowerSet<T> resultSet = firstSet;

            foreach (PowerSet<T> item in sets)
                resultSet = resultSet.Intersection(item);

            return resultSet;
        }
    }
}
