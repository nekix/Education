using System;
using System.Collections.Generic;

namespace Ads.Exercise9
{
    public class OrderedListBasedDictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        private readonly SortedList<TKey, TValue> _list;

        public OrderedListBasedDictionary(int initalSize)
        {
            _list = new SortedList<TKey, TValue>(initalSize);
        }

        public bool IsKey(TKey key)
            => _list.ContainsKey(key);

        public void Put(TKey key, TValue value)
            => _list[key] = value;

        public TValue Get(TKey key)
            => _list[key];

        public void Delete(TKey key)
            => _list.Remove(key);
    }
}
