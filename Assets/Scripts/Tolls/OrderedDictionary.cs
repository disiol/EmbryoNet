using System;
using System.Collections;
using System.Collections.Generic;
using Models;

namespace Tolls
{
    public class OrderedDictionary<TKey, TValue> : IEnumerable
    {
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        private List<TKey> insertionOrder = new List<TKey>();

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            insertionOrder.Add(key);
        }

        public int GetPosition(TKey key)
        {
            return insertionOrder.IndexOf(key);
        }

        public TValue GetValueAtPosition(int position)
        {
            if (position >= 0 && position < insertionOrder.Count)
            {
                TKey key = insertionOrder[position];
                return dictionary[key];
            }
            throw new ArgumentOutOfRangeException(nameof(position));
        }


        public TValue this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}