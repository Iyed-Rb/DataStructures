using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

/*     
 The SortedList<TKey, TValue> in .NET uses two separate arrays—one for keys and one for values—instead of
a single array of key-value pairs (like structs) to optimize performance and memory efficiency.
This design allows the keys array to remain strictly sorted, enabling fast binary search operations on just the keys. 
When inserting or removing elements, only the keys and values need to be shifted independently, 
which is cheaper than moving whole key-value structs, especially when values are large or complex objects.
Separating keys and values also improves cache locality, since operations on keys
(like comparisons and searches) work on a contiguous block of uniform data. Overall,
this approach leads to faster lookups and more efficient memory usage compared to storing pairs together.
 */


public class clsSortedList <TKey, TValue> : IDictionary<TKey, TValue>
{

    private List<TKey> _keys;
    private List<TValue> _values;
    private IComparer<TKey> _comparer;
    private int _count;

    public clsSortedList(int capacity = 4, IComparer<TKey> comparer = null)
    {
        _keys = new List<TKey>(capacity);
        _values = new List<TValue>(capacity);
        _count = 0;
        _comparer = comparer ?? Comparer<TKey>.Default;
    }

    public int Count => _count;

    private int FindKeyIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = _keys.BinarySearch(key, _comparer);
        return index; // >=0 if found, <0 if not found (bitwise complement of insertion point)
    }

    public void Add(TKey key, TValue value)
    {

        int index = FindKeyIndex(key);

        if (index >= 0)
        {
            throw new ArgumentException("Key already exists");

        }


        int insertIndex = ~index;

        _keys.Insert(insertIndex, key);
        _values.Insert(insertIndex, value);
        _count++;

    }

    public bool Remove(TKey key)
    {

        int index = FindKeyIndex(key);
        if (index < 0)
            return false; // key not found

        _keys.RemoveAt(index);
        _values.RemoveAt(index);
        _count--;
        return true; // removed successfully
    }


    public TValue this[TKey key]
    {

        get
        {

            int index = FindKeyIndex(key);
            if (index < 0)
                throw new KeyNotFoundException();
            return _values[index];
        }
        set
        {
            int index = FindKeyIndex(key);
            if (index >= 0)
            {
                _values[index] = value; // update
            }
            else
            {
                // Insert new key-value pair
                int insertIndex = ~index;
                _keys.Insert(insertIndex, key);
                _values.Insert(insertIndex, value);
                _count++;
            }
        }
    }

    public bool ContainsKey(TKey key)
    {
        return FindKeyIndex(key) >= 0;
    }

    public void Clear()
    {
        _keys.Clear();
        _values.Clear();
        _count = 0;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = FindKeyIndex(key);
        if (index >= 0)
        {
            value = _values[index];
            return true;
        }
        else
        {
            value = default(TValue);
            return false;
        }
    }

    public int IndexOfKey(TKey key)
    {
        int index = FindKeyIndex(key);
        if (index < 0)
            throw new KeyNotFoundException();

        // Return the index of the key
        return index;
    }
    public int IndexOfValue(TValue value)
    {
        int index = _values.IndexOf(value);
        if (index < 0)
            throw new KeyNotFoundException("Value not found");

        // Return the index of the value
        return index;
    
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // Calls the generic version
        return GetEnumerator();
    }

    public ICollection<TKey> Keys => _keys.AsReadOnly();

    public ICollection<TValue> Values => _values.AsReadOnly();

    public bool IsReadOnly => false;

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindKeyIndex(item.Key);
        return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if ((array.Length - arrayIndex) < _count)
            throw new ArgumentException("The destination array has insufficient space.");


        for (int i = 0; i < _count; i++)
        {
            array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int index = FindKeyIndex(item.Key);
        if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], item.Value))
        {
            _keys.RemoveAt(index);
            _values.RemoveAt(index);
            _count--;
            return true;
        }
        return false;
    }

}


internal class Program
{
    static void Main(string[] args)
    {
        var sortedList = new clsSortedList<int, string>();

        // Add items
        sortedList.Add(3, "Three");
        sortedList.Add(1, "One");
        sortedList.Add(2, "Two");

        Console.WriteLine("Count after adding 3 items: " + sortedList.Count);

        // Iterate (should be sorted by key)
        Console.WriteLine("Items in sorted order:");
        foreach (var kvp in sortedList)
        {
            Console.WriteLine($"{kvp.Key} => {kvp.Value}");
        }

        // Test indexer get/set
        Console.WriteLine($"Value for key 2: {sortedList[2]}");
        sortedList[2] = "Two Updated";
        Console.WriteLine($"Updated value for key 2: {sortedList[2]}");

        // Test ContainsKey and TryGetValue
        Console.WriteLine($"Contains key 3? {sortedList.ContainsKey(3)}");
        if (sortedList.TryGetValue(3, out string val))
        {
            Console.WriteLine($"TryGetValue for key 3: {val}");
        }

        // Test Remove by key
        bool removed = sortedList.Remove(1);
        Console.WriteLine($"Removed key 1: {removed}");
        Console.WriteLine("Items after removal:");
        foreach (var kvp in sortedList)
        {
            Console.WriteLine($"{kvp.Key} => {kvp.Value}");
        }

        // Test Remove by KeyValuePair
        var pair = new KeyValuePair<int, string>(2, "Two Updated");
        bool removedPair = sortedList.Remove(pair);
        Console.WriteLine($"Removed pair {pair.Key} => {pair.Value}: {removedPair}");

        // Final count
        Console.WriteLine("Final count: " + sortedList.Count);

        // Clear all
        sortedList.Clear();
        Console.WriteLine("Count after Clear: " + sortedList.Count);

        Console.ReadKey();  
    }
}

