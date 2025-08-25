using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class HashEntry<TKey, TValue>
{
    public int HashCode;
    public TKey Key;
    public TValue Value;
    public HashEntry<TKey, TValue> Next;

    public HashEntry(int hashCode, TKey key, TValue value)
    {
        HashCode = hashCode;
        Key = key;
        Value = value;
        Next = null;
    }
}

public class MyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private HashEntry<TKey, TValue>[] buckets;
    private int count;
    private int initialCapacity = 4;

    public int Count => count;

    public MyDictionary(int size = 4)
    {
        buckets = new HashEntry<TKey, TValue>[size];
        count = 0;
        initialCapacity = size;
    }

    private void Insert(TKey key, TValue value, bool overwrite)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        if (!overwrite && (float)(count + 1) / buckets.Length >= 0.75f)
        {
            Resize();
        }

        int hash = key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;

        HashEntry<TKey, TValue> current = buckets[index];
        HashEntry<TKey, TValue> previous = null;

        while (current != null)
        {
            if (current.Key.Equals(key))
            {
                if (!overwrite)
                    throw new ArgumentException("An item with the same key already exists.");

                current.Value = value;
                return;
            }

            previous = current;
            current = current.Next;
        }

        var newEntry = new HashEntry<TKey, TValue>(hash, key, value);
        if (previous == null)
            buckets[index] = newEntry;
        else
            previous.Next = newEntry;

        count++;
    }

    public void Add(TKey key, TValue value) => Insert(key, value, overwrite: false);

    public bool ContainsKey(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;

        var current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(key))
                return true;
            current = current.Next;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;

        var current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(key))
            {
                value = current.Value;
                return true;
            }
            current = current.Next;
        }

        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            int hash = key.GetHashCode();
            int index = (hash & 0x7FFFFFFF) % buckets.Length;
            var current = buckets[index];
            while (current != null)
            {
                if (current.Key.Equals(key))
                    return current.Value;
                current = current.Next;
            }
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }
        set => Insert(key, value, overwrite: true);
    }

    public bool Remove(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;

        var current = buckets[index];
        HashEntry<TKey, TValue> previous = null;

        while (current != null)
        {
            if (current.Key.Equals(key))
            {
                if (previous == null)
                {
                    buckets[index] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                count--;
                return true;
            }
            previous = current;
            current = current.Next;
        }
        return false;
    }

    public void Clear()
    {
        buckets = new HashEntry<TKey, TValue>[initialCapacity];
        count = 0;
    }

    private void Resize()
    {
        int newSize = buckets.Length * 2;
        var oldBuckets = buckets;
        buckets = new HashEntry<TKey, TValue>[newSize];

        foreach (var entry in oldBuckets)
        {
            var current = entry;
            while (current != null)
            {
                int index = (current.Key.GetHashCode() & 0x7FFFFFFF) % newSize;
                var newEntry = new HashEntry<TKey, TValue>(current.HashCode, current.Key, current.Value);
                newEntry.Next = buckets[index];
                buckets[index] = newEntry;
                current = current.Next;
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(count);
            foreach (var bucket in buckets)
            {
                var current = bucket;
                while (current != null)
                {
                    keys.Add(current.Key);
                    current = current.Next;
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>(count);
            foreach (var bucket in buckets)
            {
                var current = bucket;
                while (current != null)
                {
                    values.Add(current.Value);
                    current = current.Next;
                }
            }
            return values;
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue val))
        {
            return EqualityComparer<TValue>.Default.Equals(val, item.Value);
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var pair in this)
        {
            if (arrayIndex >= array.Length) break;
            array[arrayIndex++] = pair;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
            return Remove(item.Key);
        return false;
    }

    public bool IsReadOnly => false;
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in buckets)
        {
            var current = bucket;
            while (current != null)
            {
                yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                current = current.Next;
            }
        }
    }

    // Explicit non-generic enumerator (no 'public')
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}


internal class Program
{
    static void Main(string[] args)
    {
        var dict = new MyDictionary<string, int>();

        // Add entries
        dict.Add("apple", 5);
        dict.Add("banana", 3);
        dict["orange"] = 7;  // Using indexer to add/update

        // Access values
        Console.WriteLine($"apple: {dict["apple"]}");
        Console.WriteLine($"banana: {dict["banana"]}");
        Console.WriteLine($"orange: {dict["orange"]}");

        // Update value
        dict["banana"] = 10;
        Console.WriteLine($"Updated banana: {dict["banana"]}");

        // ContainsKey and TryGetValue
        Console.WriteLine($"Contains 'apple': {dict.ContainsKey("apple")}");
        if (dict.TryGetValue("grape", out int grapeCount))
            Console.WriteLine($"Grape count: {grapeCount}");
        else
            Console.WriteLine("Grape not found");

        // Remove an item
        dict.Remove("apple");
        Console.WriteLine($"Contains 'apple' after removal: {dict.ContainsKey("apple")}");

        // Iterate with foreach (uses your generic enumerator)
        Console.WriteLine("\nAll items in dictionary:");
        foreach (var kvp in dict)
        {
            Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }

        Console.WriteLine($"\nTotal Count: {dict.Count}");

        Console.ReadLine();
    }
}

