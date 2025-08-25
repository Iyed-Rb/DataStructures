using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyHashSet<T> : ICollection<T>, IEnumerable<T>, ISet<T>
{
    private class Node
    {
        public T Value;
        public Node Next;

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node[] buckets;
    private int count;
    private const float LoadFactor = 0.75f;

    public MyHashSet(int capacity = 16)
    {
        if (capacity <= 0) capacity = 16;
        buckets = new Node[capacity];
        count = 0;
    }

    public MyHashSet(IEnumerable<T> collection)
    : this() // Calls the default constructor to set up buckets, etc...
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }
    public int Count => count;
    public bool IsReadOnly => false;

    public bool Add(T item)
    {
        if (Contains(item))
            return false;

        if (count >= buckets.Length * LoadFactor)
        {
            Resize();
        }

        int index = GetBucketIndex(item);
        Node newNode = new Node(item);
        newNode.Next = buckets[index]; 
        buckets[index] = newNode;
        count++;
        return true;
    }

    void ICollection<T>.Add(T item) => Add(item);

    public bool Contains(T item)
    {
        int index = GetBucketIndex(item);
        Node current = buckets[index];
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        while (current != null)
        {
            if (comparer.Equals(current.Value, item))
                return true;
            current = current.Next;
        }

        return false;
    }

    public bool Remove(T item)
    {
        int index = GetBucketIndex(item);
        Node current = buckets[index];
        Node prev = null;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        while (current != null)
        {
            if (comparer.Equals(current.Value, item))
            {
                if (prev == null)
                    buckets[index] = current.Next;
                else
                    prev.Next = current.Next;

                count--;
                return true;
            }
            prev = current;
            current = current.Next;
        }

        return false;
    }

    public void Clear()
    {
        buckets = new Node[buckets.Length];
        count = 0;
    }

    private void Resize()
    {
        Node[] oldBuckets = buckets;
        buckets = new Node[oldBuckets.Length * 2];
        count = 0;

        foreach (var node in oldBuckets)
        {
            Node current = node;
            while (current != null)
            {
                Add(current.Value);
                current = current.Next;
            }
        }
    }

    private int GetBucketIndex(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        int hash = item.GetHashCode() & 0x7FFFFFFF; // remove sign bit
        return hash % buckets.Length;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var node in buckets)
        {
            Node current = node;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (ReferenceEquals(this, other))
            return;

        foreach (var item in other)
        {
            this.Add(item); // Add already handles duplicates
        }
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (ReferenceEquals(this, other))
            return;

        // Create a temporary hash set for faster lookup in 'other'
        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        var result = new MyHashSet<T>();

        //foreach (var item in this)
        //{
        //    foreach (var item2 in other)
        //    {
        //        if (item.Equals(item2))
        //        {
        //            result.Add(item);
        //            break; // no need to keep checking once we found a match
        //        }
        //    }
        //}  ******** BAD For Time Complexity ********** 

        

        foreach (var item in this)
        {
            if (other.Contains(item))
            {
                result.Add(item);
            }
        }

        this.Clear();
        foreach (var item in result)
        {
            this.Add(item);
        }

    }

    public void ExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (ReferenceEquals(this, other))
        {
            this.Clear();
            return;
        }

        // Use a hash set for fast lookup
        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        var toRemove = new List<T>();

        foreach (var item in this)
        {
            if (other.Contains(item))
            {
                toRemove.Add(item);

            }
        }

        foreach (var item in toRemove)
        {
            this.Remove(item);
        }
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (ReferenceEquals(this, other))
        {
            this.Clear();
            return;
        }

        // Use a hash set for fast lookup
        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        var result = new MyHashSet<T>();

        foreach (var item in this)
        {
            if (!otherSet.Contains(item))
            {
                result.Add(item);

            }
        }
        foreach (var item in other)
        {
            if (!this.Contains(item))
            {
                result.Add(item);

            }
        }

        this.Clear();
        foreach (var item in result)
        {
            this.Add(item);
        }
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (ReferenceEquals(this, other))
            return true;

        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        if (this.Count != otherSet.Count)
            return false;

        foreach (var item in this)
        {
            if (!otherSet.Contains(item))
                return false;
        }

        return true;
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        foreach (var item in this)
        {
            if (!otherSet.Contains(item))
                return false;
        }

        return true;
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        foreach (var item in other)
        {
            if (!this.Contains(item))
                return false;
        }

        return true;
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        // Optional: convert 'other' to hash set if not already
        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        foreach (var item in this)
        {
            if (otherSet.Contains(item))
                return true;
        }

        return false;
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        MyHashSet<T> otherSet = other as MyHashSet<T> ?? new MyHashSet<T>(other);

        if (this.Count >= otherSet.Count)
            return false; // can't be proper subset if size >= other

        foreach (var item in this)
        {
            if (!otherSet.Contains(item))
                return false;
        }

        return true;
    }


    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        // For efficiency, if 'other' is empty, 'this' must have at least one element to be proper superset
        int otherCount = 0;
        foreach (var _ in other)
            otherCount++;

        if (this.Count <= otherCount)
            return false; // can't be proper superset if size <= other

        foreach (var item in other)
        {
            if (!this.Contains(item))
                return false;
        }

        return true;
    }

}

class Program
{
    static void Main()
    {
        MyHashSet<int> set1 = new MyHashSet<int> { 1, 2, 3 };
        MyHashSet<int> set2 = new MyHashSet<int> { 3, 4, 5 };
        MyHashSet<int> set3 = new MyHashSet<int> { 1, 2, 3, 4, 5 };

        Console.WriteLine("Initial sets:");
        Console.WriteLine("set1: " + string.Join(", ", set1));
        Console.WriteLine("set2: " + string.Join(", ", set2));
        Console.WriteLine("set3: " + string.Join(", ", set3));

        // Union
        var union = new MyHashSet<int>(set1);
        union.UnionWith(set2);
        Console.WriteLine("Union (set1 ∪ set2): " + string.Join(", ", union));

        // Intersection
        var intersection = new MyHashSet<int>(set1);
        intersection.IntersectWith(set2);
        Console.WriteLine("Intersection (set1 ∩ set2): " + string.Join(", ", intersection));

        // Except (set1 - set2)
        var except = new MyHashSet<int>(set1);
        except.ExceptWith(set2);
        Console.WriteLine("Except (set1 - set2): " + string.Join(", ", except));

        // SymmetricExceptWith
        var symExcept = new MyHashSet<int>(set1);
        symExcept.SymmetricExceptWith(set2);
        Console.WriteLine("Symmetric Except (set1 Δ set2): " + string.Join(", ", symExcept));

        // SetEquals
        Console.WriteLine("set1 SetEquals set2? " + set1.SetEquals(set2));
        Console.WriteLine("set1 SetEquals set1? " + set1.SetEquals(set1));

        // IsSubsetOf
        Console.WriteLine("set1 IsSubsetOf set3? " + set1.IsSubsetOf(set3));
        Console.WriteLine("set2 IsSubsetOf set1? " + set2.IsSubsetOf(set1));

        // IsProperSubsetOf
        Console.WriteLine("set1 IsProperSubsetOf set3? " + set1.IsProperSubsetOf(set3));
        Console.WriteLine("set3 IsProperSubsetOf set1? " + set3.IsProperSubsetOf(set1));

        // IsSupersetOf
        Console.WriteLine("set3 IsSupersetOf set1? " + set3.IsSupersetOf(set1));
        Console.WriteLine("set1 IsSupersetOf set2? " + set1.IsSupersetOf(set2));

        // IsProperSupersetOf
        Console.WriteLine("set3 IsProperSupersetOf set1? " + set3.IsProperSupersetOf(set1));
        Console.WriteLine("set1 IsProperSupersetOf set3? " + set1.IsProperSupersetOf(set3));

        // Overlaps
        Console.WriteLine("set1 Overlaps set2? " + set1.Overlaps(set2));
        Console.WriteLine("set1 Overlaps except? " + set1.Overlaps(except));

        // Test Add, Remove, Contains with strings
        var testSet = new MyHashSet<string>();
        testSet.Add("apple");
        testSet.Add("banana");
        Console.WriteLine("Contains 'banana'? " + testSet.Contains("banana"));
        testSet.Remove("banana");
        Console.WriteLine("Contains 'banana' after removal? " + testSet.Contains("banana"));

        Console.ReadKey();
    }
}
