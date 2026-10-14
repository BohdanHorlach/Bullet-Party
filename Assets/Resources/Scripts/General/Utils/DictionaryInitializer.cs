using System;
using System.Collections.Generic;
using UnityEngine;


namespace General.Utils
{
    [Serializable]
    /// <summary>
    /// Provides utility methods for initializing dictionaries from arrays or collections.
    /// </summary>
    public class DictionaryInitializer
    {
        /// <summary>
        /// Creates a dictionary from an array of items using the specified key and value selector functions.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input array.</typeparam>
        /// <typeparam name="K">The type of keys in the resulting dictionary.</typeparam>
        /// <typeparam name="V">The type of values in the resulting dictionary.</typeparam>
        /// <param name="list">The array of elements to convert into a dictionary</param>
        /// <param name="keySelector">A function that selects the key for each element.</param>
        /// <param name="valueSelector">A function that selects the value for each element.</param>
        /// <returns>
        /// A dictionary where each element from the input array is represented as a key-value pair.
        /// If duplicate keys are found, a warning is logged and the first occurrence is retained.
        /// </returns>
        public static Dictionary<K, V> InstantiateDictionary<T, K, V>(T[] list, Func<T, K> keySelector, Func<T, V> valueSelector)
        {
            Dictionary<K, V>  dictionary = new Dictionary<K, V>();

            foreach (var item in list)
            {
                K key = keySelector(item);
                V value = valueSelector(item);

                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, value);
                else
                    Debug.LogWarning($"Duplicate key found: {key}");
            }

            return dictionary;
        }
    }
}