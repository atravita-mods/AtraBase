using System.Collections;
using AtraBase.Toolkit;
#if !NET6_0_OR_GREATER // IEnumerable<T>.Min(comparer) exists in LINQ in NET6
using AtraBase.Toolkit.Shims.NetSix;
#endif
using CommunityToolkit.Diagnostics;

namespace AtraBase.Collections;

/// <summary>
/// A binary heap.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
/// <remarks>Consulted the net 5 implementation of a binary heap and also python's heapq package
/// while writing this.
///
/// Dotnet impl: https://referencesource.microsoft.com/#PresentationCore/Shared/MS/Internal/PriorityQueue.cs
/// Python impl: https://github.com/python/cpython/blob/3.10/Lib/heapq.py .</remarks>
public class BiHeap<T> : ICollection<T>
    where T : notnull, IEquatable<T>
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Reviewed.")]
    private const int DEFAULT_CAPACITY = 6;

    private readonly IComparer<T> comparer;
    private int count = 0;
    private T[] heap;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiHeap{T}"/> class.
    /// </summary>
    public BiHeap()
    {
        this.heap = new T[DEFAULT_CAPACITY];
        this.comparer = Comparer<T>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiHeap{T}"/> class.
    /// </summary>
    /// <param name="capacity">Initial capacity.</param>
    public BiHeap(int capacity)
    {
        Guard.IsGreaterThan(capacity, 0);

        this.heap = GC.AllocateUninitializedArray<T>(capacity);
        this.comparer = Comparer<T>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiHeap{T}"/> class.
    /// </summary>
    /// <param name="capacity">Initial capacity.</param>
    /// <param name="comparer">Comparer to use.</param>
    public BiHeap(int capacity, IComparer<T> comparer)
    {
        Guard.IsGreaterThan(capacity, 0);
        Guard.IsNotNull(comparer);

        this.heap = new T[capacity];
        this.comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiHeap{T}"/> class with a default comparer.
    /// </summary>
    /// <param name="values">Initial values.</param>
    public BiHeap(IEnumerable<T> values)
    {
        Guard.IsNotNull(values);

        this.ReadInIEnumerable(values);
        this.comparer = Comparer<T>.Default;
        this.Heapify();
    }

    public BiHeap(IEnumerable<T> values, IComparer<T> comparer)
    {
        Guard.IsNotNull(values);
        Guard.IsNotNull(comparer);

        this.ReadInIEnumerable(values);
        this.comparer = comparer;

        this.Heapify();
    }

    /// <inheritdoc />
    public int Count => this.count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    public static List<TType> NthSmallest<TType>(int n, IEnumerable<TType> items, IComparer<TType>? comparer = null)
        where TType : notnull, IEquatable<TType>
    {
        comparer ??= Comparer<TType>.Default;

        // Only looking for one element. Just use Min.
        if (n == 1)
        {
            TType? min = items.Min(comparer);
            if (min is null)
            {
                return new List<TType>();
            }
            return new List<TType>() { min };
        }

        // Number of elements wanted is larger than the number of elements in the collection.
        if (items is ICollection collection && collection.Count <= n)
        {
            List<TType> ret = new(items);
            ret.Sort(comparer);
            return ret;
        }

        {
            BiHeap<TType> heap = new(items, comparer);
            List<TType> ret = new(n);
            while (n-- > 0 && heap.TryPop(out TType? val))
            {
                ret.Add(val);
            }
            return ret;
        }
    }

    public void Push(T item)
        => this.Add(item);

    public void Add(T item)
    {
        Guard.IsNotNull(item);

        this.ExpandHeapIfNeeded();
        this.SiftUp(this.count, 0, ref item);
        this.count++;
    }

    public void AddRange(IList<T> items)
    {
        Guard.IsNotNull(items);

        this.ExpandHeapIfNeeded(items.Count);

        if (this.count / 2 < items.Count)
        {
            // there's a lot of new items. I'm better off
            // just combining and re-heaping.
            items.CopyTo(this.heap, this.count);
            this.count += items.Count;
            this.Heapify();
        }
        else
        {
            // There's not actually that many new items.
            // Just sift each one up.
            for (int i = 0; i < items.Count; i++)
            {
                T item = items[i];
                this.SiftUp(this.count, 0, ref item);
                this.count++;
            }
        }
    }

    public T Peek()
        => this.heap[0];

    public bool TryPeek([NotNullWhen(true)] out T? val)
    {
        if (this.count == 0)
        {
            val = default;
            return false;
        }
        val = this.heap[0];
        return true;
    }

    public T Pop()
    {
        if (this.TryPop(out T? val))
        {
            return val;
        }

        return TKThrowHelper.ThrowIndexOutOfRangeException<T>();
    }

    public bool TryPop([NotNullWhen(true)] out T? val)
    {
        if (this.count == 0)
        {
            val = default;
            return false;
        }
        this.count--;
        val = this.heap[0];
        T? last = this.heap[this.count];
        this.heap[this.count] = default!;
        int leaf = this.SiftGapDown(0);
        this.SiftUp(leaf, 0, ref last);

        return true;
    }

    /// <summary>
    /// Pops the minimum item from the heap and pushes a new value into the heap.
    /// </summary>
    /// <param name="item">New value to push.</param>
    /// <returns>Old value.</returns>
    public T PushPop(T item)
    {
        Guard.IsNotNull(item);

        if (this.count == 0)
        {
            return item;
        }

        return this.Replace(item);
    }

    public T Replace(T item)
    {
        Guard.IsNotNull(item);

        T? ret = this.heap[0];
        int leaf = this.SiftGapDown(0);
        this.SiftUp(leaf, 0, ref item);
        return ret;
    }

    public void Clear()
    {
        this.count = 0;
        this.heap = new T[DEFAULT_CAPACITY];
    }

    // suspect I'm not gonna get better than the O(n) search here.
    public bool Contains(T item)
        => this.heap.Contains(item);

    public bool Remove(T item)
    {
        Guard.IsNotNull(item);

        for (int i = this.count - 1; i >= 0; i--)
        {
            if (item.Equals(this.heap[i]))
            {
                int leaf = this.SiftGapDown(i);
                this.heap[leaf] = default!;
                this.count--;
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
        => this.heap.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)this;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this;

    private T? current;

    public T? Current
    {
        get => this.current;
        private set => this.current = value;
    }

    public bool MoveNext()
        => this.TryPop(out this.current);

    // right child is just left child + 1
    private static int GetLeftChild(int index) => (index * 2) + 1;

    private static int GetParent(int index) => (index - 1) / 2;

    private void Heapify()
    {
        for (int i = (this.count / 2) - 1; i >= 0; i--)
        {
            T value = this.heap[i];
            int leaf = this.SiftGapDown(i);
            this.SiftUp(leaf, i, ref value);
        }
    }

    private void ExpandHeapIfNeeded(int amount = 1)
    {
        if (this.count + amount > this.heap.Length)
        {
            Array.Resize(ref this.heap, Math.Max(this.count * 2, this.count + amount));
        }
    }

    // used for initialization, reads in an IEnumerable and sets .count and .heap
    [MemberNotNull("heap", "count")]
    private int ReadInIEnumerable(IEnumerable<T> items)
    {
        Guard.IsNotNull(items);
        Guard.IsEqualTo(this.count, 0, "This function can only be used with an empty heap");

        if (items is ICollection collection)
        {
            this.count = collection.Count;
            this.heap = GC.AllocateUninitializedArray<T>(this.Count);
            collection.CopyTo(this.heap, 0);
            return this.count;
        }
        else
        {
            this.heap = new T[DEFAULT_CAPACITY];
            this.count = 0;
            foreach (T item in items)
            {
                if (this.heap.Length == this.count)
                {
                    Array.Resize(ref this.heap, this.count * 2);
                }
                this.heap[this.count] = item;
                this.count++;
            }
            return this.count;
        }
    }

    /// <summary>
    /// Sifts a gap down to the bottom of the tree.
    /// </summary>
    /// <param name="index">The index to start at.</param>
    /// <returns>The index the gap ends up at.</returns>
    private int SiftGapDown(int index)
    {
        int end = this.count;
        int child = GetLeftChild(index);
        while (child < end)
        {
            int right = child + 1;

            // find the better child.
            child = (right < this.count && this.comparer.Compare(this.heap[right], this.heap[child]) < 0)
                ? right
                : child;

            // promote the better child.
            this.heap[index] = this.heap[child];

            index = child;
            child = GetLeftChild(index);
        }

        return index;
    }

    /// <summary>
    /// While pretending item is at index, sift it up until it fits into the heap.
    /// </summary>
    /// <param name="index">Index to start at.</param>
    /// <param name="boundary">Boundary to stop at.</param>
    /// <param name="item">The item to stash. This is a ref param to avoid excess copying.</param>
    /// <returns>Index it ended up at.</returns>
    private int SiftUp(int index, int boundary, ref T item)
    {
        while (index > boundary)
        {
            int parent = GetParent(index);

            // if the value is greater than the parent
            // move the parent down.
            if (this.comparer.Compare(this.heap[parent], item) > 0)
            {
                this.heap[index] = this.heap[parent];
                index = parent;
            }
            else
            {
                break;
            }
        }
        this.heap[index] = item;
        return index;
    }
}
