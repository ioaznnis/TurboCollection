using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TurboCollection
{
    /// <summary>
    /// Represents a collection of unique composite key (Id, Name) and values 
    /// </summary>
    /// <typeparam name="TKeyId"></typeparam>
    /// <typeparam name="TKeyName"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MultiKeyCollection<TKeyId, TKeyName, TValue> :
        IMultiKeyCollection<TKeyId, TKeyName, TValue>
        where TKeyId : notnull
        where TKeyName : notnull
    {
        private readonly Dictionary<CompositeKey<TKeyId, TKeyName>, TValue> _dictionary;
        private readonly Dictionary<TKeyId, List<CompositeKey<TKeyId, TKeyName>>> _idDictionary;
        private readonly Dictionary<TKeyName, List<CompositeKey<TKeyId, TKeyName>>> _nameDictionary;

        public MultiKeyCollection()
        {
            _dictionary = new Dictionary<CompositeKey<TKeyId, TKeyName>, TValue>();
            _idDictionary = new Dictionary<TKeyId, List<CompositeKey<TKeyId, TKeyName>>>();
            _nameDictionary = new Dictionary<TKeyName, List<CompositeKey<TKeyId, TKeyName>>>();
        }

        private void AddIndex(CompositeKey<TKeyId, TKeyName> key)
        {
            AddIndexKey(key, key.Id, _idDictionary);
            AddIndexKey(key, key.Name, _nameDictionary);

            static void AddIndexKey<TKey>(
                CompositeKey<TKeyId, TKeyName> key,
                TKey keyId,
                IDictionary<TKey, List<CompositeKey<TKeyId, TKeyName>>> dictionary)
            {
                if (dictionary.TryGetValue(keyId, out var keys))
                {
                    keys.Add(key);
                }
                else
                {
                    dictionary.Add(keyId, new List<CompositeKey<TKeyId, TKeyName>> {key});
                }
            }
        }

        private void RemoveIndex(CompositeKey<TKeyId, TKeyName> key)
        {
            RemoveIndexKey(key, key.Id, _idDictionary);
            RemoveIndexKey(key, key.Name, _nameDictionary);

            static void RemoveIndexKey<TKey>(
                CompositeKey<TKeyId, TKeyName> key,
                TKey keyId,
                IDictionary<TKey, List<CompositeKey<TKeyId, TKeyName>>> dictionary)
            {
                dictionary[keyId].Remove(key);
            }
        }

        public void Add(CompositeKey<TKeyId, TKeyName> key, TValue value)
        {
            _dictionary.Add(key, value);

            AddIndex(key);
        }

        public bool Remove(CompositeKey<TKeyId, TKeyName> key)
        {
            var remove = _dictionary.Remove(key);
            if (remove)
            {
                RemoveIndex(key);
            }

            return remove;
        }

        public bool ContainsKey(CompositeKey<TKeyId, TKeyName> key) => _dictionary.ContainsKey(key);

        public IEnumerable<TValue> GetValueById(TKeyId keyId) =>
            _idDictionary[keyId].Select(x => _dictionary[x]);

        public IEnumerable<TValue> GetValueByName(TKeyName keyName) => 
            _nameDictionary[keyName].Select(x => _dictionary[x]);

        public bool TryGetValue(CompositeKey<TKeyId, TKeyName> key, [MaybeNullWhen(false)] out TValue value) =>
            _dictionary.TryGetValue(key, out value);

        public TValue this[CompositeKey<TKeyId, TKeyName> key]
        {
            get => _dictionary[key];
            set
            {
                if (!_dictionary.ContainsKey(key))
                {
                    AddIndex(key);
                }

                _dictionary[key] = value;
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
            _idDictionary.Clear();
            _nameDictionary.Clear();
        }

        public ICollection<CompositeKey<TKeyId, TKeyName>> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        #region Interface implemantation

        IEnumerator<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>
            IEnumerable<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.GetEnumerator() =>
            _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _dictionary).GetEnumerator();

        void ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.Add(
            KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue> item) =>
            Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.
            Contains(KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue> item) =>
            ((ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>) _dictionary).Contains(item);

        void ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.CopyTo(
            KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>) _dictionary).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.Remove(
            KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue> keyValuePair)
        {
            if (TryGetValue(keyValuePair.Key, out var value) &&
                EqualityComparer<TValue>.Default.Equals(value, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<CompositeKey<TKeyId, TKeyName>, TValue>>.IsReadOnly => false;

        #endregion
    }
}