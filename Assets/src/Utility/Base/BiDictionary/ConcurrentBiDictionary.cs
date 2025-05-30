﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Cosmos.system
{
    [Serializable]
    public class ConcurrentBiDictionary<TFirst, TSecond> : IDictionary<TFirst, TSecond>, IReadOnlyDictionary<TFirst, TSecond>, IDictionary
    {
        private readonly IDictionary<TFirst, TSecond> _firstToSecond = new ConcurrentDictionary<TFirst, TSecond>();
        [NonSerialized]
        private readonly IDictionary<TSecond, TFirst> _secondToFirst = new ConcurrentDictionary<TSecond, TFirst>();
        [NonSerialized]
        private readonly ReverseDictionary _reverseDictionary;

        public ConcurrentBiDictionary()
        {
            _reverseDictionary = new ReverseDictionary(this);
        }
        public TSecond this[TFirst key]
        {
            get => _firstToSecond[key];
            set
            {
                _firstToSecond[key] = value;
                _secondToFirst[value] = key;
            }
        }
        object IDictionary.this[object key]
        {
            get => ((IDictionary)_firstToSecond)[key];
            set
            {
                ((IDictionary)_firstToSecond)[key] = value;
                ((IDictionary)_secondToFirst)[value] = key;
            }
        }

        public IDictionary<TSecond, TFirst> Reverse => _reverseDictionary;
        public int Count => _firstToSecond.Count;
        object ICollection.SyncRoot => ((ICollection)_firstToSecond).SyncRoot;
        bool ICollection.IsSynchronized => ((ICollection)_firstToSecond).IsSynchronized;
        bool IDictionary.IsFixedSize => ((IDictionary)_firstToSecond).IsFixedSize;
        public bool IsReadOnly => _firstToSecond.IsReadOnly || _secondToFirst.IsReadOnly;
        public ICollection<TFirst> Keys => _firstToSecond.Keys;
        ICollection IDictionary.Keys => ((IDictionary)_firstToSecond).Keys;
        IEnumerable<TFirst> IReadOnlyDictionary<TFirst, TSecond>.Keys => ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Keys;
        public ICollection<TSecond> Values => _firstToSecond.Values;
        ICollection IDictionary.Values => ((IDictionary)_firstToSecond).Values;
        IEnumerable<TSecond> IReadOnlyDictionary<TFirst, TSecond>.Values => ((IReadOnlyDictionary<TFirst, TSecond>)_firstToSecond).Values;
        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() => _firstToSecond.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_firstToSecond).GetEnumerator();

        public void Add(TFirst key, TSecond value)
        {
            _firstToSecond.Add(key, value);
            _secondToFirst.Add(value, key);
        }
        public bool TryAdd(TFirst key, TSecond value)
        {
            if (_firstToSecond.ContainsKey(key) || _secondToFirst.ContainsKey(value))
                return false;
            _firstToSecond.Add(key, value);
            _secondToFirst.Add(value, key);
            return true;
        }
        public void AddOrUpdate(TFirst key, TSecond value)
        {
            if (_firstToSecond.ContainsKey(key))
                _firstToSecond.Remove(key);
            if (_secondToFirst.ContainsKey(value))
                _secondToFirst.Remove(value);
            _firstToSecond.Add(key, value);
            _secondToFirst.Add(value, key);
        }
        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)_firstToSecond).Add(key, value);
            ((IDictionary)_secondToFirst).Add(value, key);
        }

        void ICollection<KeyValuePair<TFirst, TSecond>>.Add(KeyValuePair<TFirst, TSecond> item)
        {
            _firstToSecond.Add(item);
            _secondToFirst.Add(new KeyValuePair<TSecond, TFirst>(item.Value, item.Key));
        }

        public bool ContainsKey(TFirst key) => _firstToSecond.ContainsKey(key);
        bool ICollection<KeyValuePair<TFirst, TSecond>>.Contains(KeyValuePair<TFirst, TSecond> item) => _firstToSecond.Contains(item);
        public bool TryGetValue(TFirst key, out TSecond value) => _firstToSecond.TryGetValue(key, out value) && _secondToFirst.ContainsKey(value);
        public bool Remove(TFirst key)
        {
            TSecond value;
            if (_firstToSecond.TryGetValue(key, out value))
            {
                if (_secondToFirst.ContainsKey(value))
                {
                    _firstToSecond.Remove(key);
                    _secondToFirst.Remove(value);
                    return true;
                }
                return false;
            }
            else
                return false;
        }

        void IDictionary.Remove(object key)
        {
            var firstToSecond = (IDictionary)_firstToSecond;
            if (!firstToSecond.Contains(key))
                return;
            var value = firstToSecond[key];
            firstToSecond.Remove(key);
            ((IDictionary)_secondToFirst).Remove(value);
        }

        bool ICollection<KeyValuePair<TFirst, TSecond>>.Remove(KeyValuePair<TFirst, TSecond> item) => _firstToSecond.Remove(item);
        bool IDictionary.Contains(object key) => ((IDictionary)_firstToSecond).Contains(key);
        public void Clear()
        {
            _firstToSecond.Clear();
            _secondToFirst.Clear();
        }
        void ICollection<KeyValuePair<TFirst, TSecond>>.CopyTo(KeyValuePair<TFirst, TSecond>[] array, int arrayIndex)
        {
            _firstToSecond.CopyTo(array, arrayIndex);
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)_firstToSecond).CopyTo(array, index);
        }
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _secondToFirst.Clear();
            foreach (var item in _firstToSecond)
                _secondToFirst.Add(item.Value, item.Key);
        }

        private class ReverseDictionary : IDictionary<TSecond, TFirst>, IReadOnlyDictionary<TSecond, TFirst>, IDictionary
        {
            private readonly ConcurrentBiDictionary<TFirst, TSecond> _owner;

            public ReverseDictionary(ConcurrentBiDictionary<TFirst, TSecond> owner)
            {
                _owner = owner;
            }
            public TFirst this[TSecond key]
            {
                get => _owner._secondToFirst[key];
                set
                {
                    _owner._secondToFirst[key] = value;
                    _owner._firstToSecond[value] = key;
                }
            }

            object IDictionary.this[object key]
            {
                get => ((IDictionary)_owner._secondToFirst)[key];
                set
                {
                    ((IDictionary)_owner._secondToFirst)[key] = value;
                    ((IDictionary)_owner._firstToSecond)[value] = key;
                }
            }

            public int Count => _owner._secondToFirst.Count;
            object ICollection.SyncRoot => ((ICollection)_owner._secondToFirst).SyncRoot;
            bool ICollection.IsSynchronized => ((ICollection)_owner._secondToFirst).IsSynchronized;
            bool IDictionary.IsFixedSize => ((IDictionary)_owner._secondToFirst).IsFixedSize;
            public bool IsReadOnly => _owner._secondToFirst.IsReadOnly || _owner._firstToSecond.IsReadOnly;
            public ICollection<TSecond> Keys => _owner._secondToFirst.Keys;
            ICollection IDictionary.Keys => ((IDictionary)_owner._secondToFirst).Keys;
            IEnumerable<TSecond> IReadOnlyDictionary<TSecond, TFirst>.Keys => ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Keys;
            public ICollection<TFirst> Values => _owner._secondToFirst.Values;
            ICollection IDictionary.Values => ((IDictionary)_owner._secondToFirst).Values;
            IEnumerable<TFirst> IReadOnlyDictionary<TSecond, TFirst>.Values => ((IReadOnlyDictionary<TSecond, TFirst>)_owner._secondToFirst).Values;
            public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator() => _owner._secondToFirst.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_owner._secondToFirst).GetEnumerator();
            public void Add(TSecond key, TFirst value)
            {
                _owner._secondToFirst.Add(key, value);
                _owner._firstToSecond.Add(value, key);
            }
            void IDictionary.Add(object key, object value)
            {
                ((IDictionary)_owner._secondToFirst).Add(key, value);
                ((IDictionary)_owner._firstToSecond).Add(value, key);
            }
            void ICollection<KeyValuePair<TSecond, TFirst>>.Add(KeyValuePair<TSecond, TFirst> item)
            {
                _owner._secondToFirst.Add(item);
                _owner._firstToSecond.Add(new KeyValuePair<TFirst, TSecond>(item.Value, item.Key));
            }
            public bool ContainsKey(TSecond key) => _owner._secondToFirst.ContainsKey(key);
            bool ICollection<KeyValuePair<TSecond, TFirst>>.Contains(KeyValuePair<TSecond, TFirst> item) => _owner._secondToFirst.Contains(item);
            public bool TryGetValue(TSecond key, out TFirst value) => _owner._secondToFirst.TryGetValue(key, out value);
            public bool Remove(TSecond key)
            {
                TFirst value;
                if (_owner._secondToFirst.TryGetValue(key, out value))
                {
                    _owner._secondToFirst.Remove(key);
                    _owner._firstToSecond.Remove(value);
                    return true;
                }
                else
                    return false;
            }

            void IDictionary.Remove(object key)
            {
                var firstToSecond = (IDictionary)_owner._secondToFirst;
                if (!firstToSecond.Contains(key))
                    return;
                var value = firstToSecond[key];
                firstToSecond.Remove(key);
                ((IDictionary)_owner._firstToSecond).Remove(value);
            }

            bool ICollection<KeyValuePair<TSecond, TFirst>>.Remove(KeyValuePair<TSecond, TFirst> item) => _owner._secondToFirst.Remove(item);
            bool IDictionary.Contains(object key) => ((IDictionary)_owner._secondToFirst).Contains(key);
            public void Clear()
            {
                _owner._secondToFirst.Clear();
                _owner._firstToSecond.Clear();
            }
            void ICollection<KeyValuePair<TSecond, TFirst>>.CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex)
            {
                _owner._secondToFirst.CopyTo(array, arrayIndex);
            }
            void ICollection.CopyTo(Array array, int index)
            {
                ((IDictionary)_owner._secondToFirst).CopyTo(array, index);
            }
        }
    }
}