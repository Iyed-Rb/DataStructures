using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

public class HashEntry
{
    public int HashCode;
    public object Key;
    public object Value;
    public HashEntry Next;

    public HashEntry(int hashCode, object key, object value)
    {
        HashCode = hashCode;
        Key = key;
        Value = value;
        Next = null;
    }
}

public class MyHashTable : IDictionary
{
    private HashEntry[] buckets;
    private int count;
    private int initialCapacity = 4; // Default initial capacity
    public int Count => count;

    public MyHashTable(int size = 4)
    {
        buckets = new HashEntry[size];
        count = 0;
        initialCapacity = size; // inside constructor

    }

    private void Insert(object key, object value, bool overwrite)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (!overwrite && (float)(count + 1) / buckets.Length >= 0.75f)
        {
            Resize();
        }

        int hash = key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;

        HashEntry current = buckets[index];
        HashEntry previous = null;

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

        var newEntry = new HashEntry(hash,key, value);
        if (previous == null)
            buckets[index] = newEntry;
        else
            previous.Next = newEntry;

        count++;
    }
    public void Add(object key, object value)
    {

        Insert(key, value, overwrite: false);
        

    }



    public object Get(object Key)
    {
        if (Key == null)
            throw new ArgumentNullException(nameof(Key));



        int hash = Key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;


        HashEntry current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(Key))
                return current.Value;

            current = current.Next;
        }

        throw new KeyNotFoundException($"Key '{Key}' not found in the hash table.");
    }

    //safe alternative to Get, returns a boolean instead of throwing an exception.
    public bool TryGetValue(object Key, out object Value)
    {
        if (Key == null)
            throw new ArgumentNullException(nameof(Key));

        int hash = Key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;


        HashEntry current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(Key))
            {
                Value = current.Value;
                return true;
            }
            current = current.Next;
        }

        Value = null;
        return false;
    }

    public void Remove(object Key)
    {
        if (Key == null)
            throw new ArgumentNullException(nameof(Key));


        int hash = Key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;


        HashEntry current = buckets[index];
        HashEntry previous = null;

        while (current != null)
        {
            if (current.Key.Equals(Key))
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
                return;
            }

            previous = current;
            current = current.Next;
        }
    }

    public bool ContainsKey(object Key)
    {
        if (Key == null)
            throw new ArgumentNullException(nameof(Key));


        int hash = Key.GetHashCode();
        int index = (hash & 0x7FFFFFFF) % buckets.Length;


        HashEntry current = buckets[index];
        while (current != null)
        {
            if (current.Key.Equals(Key))
                return true;

            current = current.Next;
        }

        return false;
    }

    public void PrintAll()
    {
        for (int i = 0; i < buckets.Length; i++)
        {
            HashEntry current = buckets[i];
            while (current != null)
            {
                Console.WriteLine($"Key: {current.Key}, Value: {current.Value}");
                current = current.Next;
            }
        }
    }

    public void Clear()
    {
        buckets = new HashEntry[initialCapacity]; 
        count = 0;
    }


    private void Resize()
    {
        int newSize = buckets.Length * 2;
        HashEntry[] oldBuckets = buckets;
        buckets = new HashEntry[newSize];

        foreach (var entry in oldBuckets)
        {
            HashEntry current = entry;
            while (current != null)
            {
                int index = (current.Key.GetHashCode() & 0x7FFFFFFF) % newSize;
                HashEntry newEntry = new HashEntry(current.HashCode,current.Key, current.Value);
                newEntry.Next = buckets[index];
                buckets[index] = newEntry;

                current = current.Next;
            }
        }
    }


    public bool Contains(object key) => ContainsKey(key);
    public object this[object key]
    {
        get => Get(key);
        set => Insert(key, value, overwrite: true);
    }

    public bool IsFixedSize => false;
    public bool IsReadOnly => false;
    public ICollection Keys => GetKeys();
    public ICollection Values => GetValues();
    private ICollection GetKeys()
    {
        List<object> keys = new List<object>(count); // reserve space for exactly count items
        foreach (var bucket in buckets)
        {
            HashEntry current = bucket;
            while (current != null)
            {
                keys.Add(current.Key);
                current = current.Next;
            }
        }
        return keys;
    }

    private ICollection GetValues()
    {
        List<object> values = new List<object>(count);
        foreach (var bucket in buckets)
        {
            HashEntry current = bucket;
            while (current != null)
            {
                values.Add(current.Value);
                current = current.Next;
            }
        }
        return values;
    }

    public object SyncRoot => this;
    public bool IsSynchronized => false;
    // IsSynchronized = false means not thread-safe; SyncRoot = this lets other code lock the hashtable if needed.
    // SyncRoot is the object used for locking when someone wants to make thread-safe access externally.


    public void CopyTo(Array array, int index)
    {
        foreach (DictionaryEntry entry in this)
        {
            array.SetValue(entry, index++);
        }
    }

    IDictionaryEnumerator IDictionary.GetEnumerator() => GetDictionaryEnumerator();

    public IEnumerator GetEnumerator() => ((IDictionary)this).GetEnumerator();

    public IDictionaryEnumerator GetDictionaryEnumerator()
    {
        List<DictionaryEntry> entries = new List<DictionaryEntry>();
        foreach (var bucket in buckets)
        {
            HashEntry current = bucket;
            while (current != null)
            {
                entries.Add(new DictionaryEntry(current.Key, current.Value));
                current = current.Next;
            }
        }

        return new MyHashtableEnumerator(entries);
    }


    // ****************** More About this ***********************
    /*   
       "Are there types of enumerators? Like the normal one for lists, and IDictionaryEnumerator for hashtables?"

       ✅ Yes! In C#, different types of enumerators exist depending on the collection:

       - Lists (List<T>, arrays, etc.) implement IEnumerator<T> or IEnumerator — they return individual values.
       - Dictionaries return key/value pairs and use:
           - IEnumerator<KeyValuePair<TKey, TValue>> (for generic Dictionary<TKey, TValue>)
           - IDictionaryEnumerator (for non-generic Hashtable or custom dictionary-like classes)

       🔧 In our custom HashTable, we implement IDictionaryEnumerator to return DictionaryEntry (with .Key and .Value).

       🔁 But note: foreach loops only call the default IEnumerator from IEnumerable, not IDictionaryEnumerator.

       So we must **forward the call** like this:
           public IEnumerator GetEnumerator() => ((IDictionary)this).GetEnumerator();

       This ensures that foreach uses our IDictionary implementation — which returns an IDictionaryEnumerator.

       🔎 Important:
       - Foreach will not automatically use IDictionaryEnumerator unless the object is explicitly cast to IDictionary.
       - To explicitly use IDictionaryEnumerator outside of foreach:
           ((IDictionary)myHashTable).GetEnumerator();

       💡 Summary:
       - IEnumerable → IEnumerator → for value-only collections
       - IDictionary → IDictionaryEnumerator → for key/value collections
    */



    private class MyHashtableEnumerator : IDictionaryEnumerator
    {
        private readonly List<DictionaryEntry> _entries;
        private int _position = -1;

        public MyHashtableEnumerator(List<DictionaryEntry> entries)
        {
            _entries = entries;
        }
        public bool MoveNext() => ++_position < _entries.Count;
        public void Reset() => _position = -1;
        public DictionaryEntry Entry => _entries[_position];
        public object Key => _entries[_position].Key;
        public object Value => _entries[_position].Value;
        public object Current => Entry;
    }


}


internal class Program
{
    static void Main(string[] args)
    {
        var table = new MyHashTable();

        Console.WriteLine("=== Add & Access ===");
        table.Add("Name", "Iyed");
        table.Add("Age", 25);
        Console.WriteLine($"Name: {table["Name"]}");
        Console.WriteLine($"Age: {table["Age"]}");

        Console.WriteLine("\n=== Update ===");
        table["Name"] = "Iyed Rabia";
        Console.WriteLine($"Updated Name: {table["Name"]}");

        Console.WriteLine("\n=== ContainsKey & TryGetValue ===");
        Console.WriteLine($"Contains 'Age': {table.Contains("Age")}");
        if (table.TryGetValue("Age", out object age))
            Console.WriteLine($"TryGetValue Age: {age}");

        Console.WriteLine("\n=== Remove ===");
        table.Remove("Age");
        Console.WriteLine($"Contains 'Age' after remove: {table.Contains("Age")}");

        Console.WriteLine("\n=== Keys & Values ===");
        Console.WriteLine("Keys: " + string.Join(", ", table.Keys.Cast<object>()));
        Console.WriteLine("Values: " + string.Join(", ", table.Values.Cast<object>()));

        Console.WriteLine("\n=== Foreach (IEnumerable) ===");
        foreach (DictionaryEntry entry in table)
        {
            Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
        }

        Console.WriteLine($"\nTotal Count: {table.Count}");
        Console.ReadLine();
    }
}



