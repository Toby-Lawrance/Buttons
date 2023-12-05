using System.Collections;

namespace ButtonsWebClient
{
    public class OrderedBag<TValue> : ICollection<TValue>, IEnumerable<TValue>, ICloneable
    {
        private readonly IComparer<TValue> comparer;
        private RedBlackTree<TValue> tree;

        public OrderedBag(IComparer<TValue> comparer)
        {
            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            this.comparer = comparer;

            tree = new RedBlackTree<TValue>(comparer);
        }

        public int Count => tree.ElementCount;

        public bool IsReadOnly => false;

        public void Add(TValue item)
        {
            tree.Insert(item, DuplicatePolicy.InsertLast, out _);
        }

        public void Clear()
        {
            tree.StopEnumerations();

            tree = new RedBlackTree<TValue>(comparer);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TValue item)
        {
            return tree.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            foreach (var obj in tree)
            {
                array[arrayIndex++] = obj;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return tree.GetEnumerator();
        }

        public bool Remove(TValue item)
        {
            return tree.Delete(item, true, out _);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}