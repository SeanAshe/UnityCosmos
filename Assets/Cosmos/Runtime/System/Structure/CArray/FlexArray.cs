using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.System
{
    public class FlexArray<T> : IEnumerable<T>
    {
        private readonly T[] _items;
        public FlexArray(T item)
        {
            _items = new T[] { item };
        }
        public FlexArray(T[] items)
        {
            _items = items ?? Array.Empty<T>();
        }
        public static implicit operator FlexArray<T>(T item)
        {
            return new FlexArray<T>(item);
        }
        public static implicit operator FlexArray<T>(T[] items)
        {
            return new FlexArray<T>(items);
        }
        public static implicit operator T[](FlexArray<T> flex)
        {
            return flex?._items ?? Array.Empty<T>();
        }
        public static implicit operator T(FlexArray<T> flex)
        {
            if (flex is null || flex._items.Length == 0)
                return default;
            return flex._items[0];
        }
        public static bool operator ==(FlexArray<T> flex, T target)
        {
            if (flex is null || flex._items?.Length == 0)
                return false;
            return flex._items.Contains(target);
        }
        public static bool operator !=(FlexArray<T> flex, T target)
        {
            return !(flex == target);
        }
        public static bool operator ==(T target, FlexArray<T> flex)
        {
            return flex == target;
        }
        public static bool operator !=(T target, FlexArray<T> flex)
        {
            return !(flex == target);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }
        public int Length => _items.Length;
        public bool IsNullOrEmpty()
        {
            return _items is null || _items.Length == 0;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        public override string ToString()
        {
            return $"[{string.Join(", ", _items)}]";
        }
    }
}
