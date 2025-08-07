using System;
using System.Collections;
using System.Collections.Generic;

public class clsList<T> : IEnumerable<T>, ICollection<T>, IList<T>
{
    private const int _DefaultSize = 4;
    private int _Size;
    private T[] arr;
    private int _count;

    private bool _isReadOnly = false;
    public bool IsReadOnly => _isReadOnly;

    public void SetReadOnly(bool readOnly) => _isReadOnly = readOnly;

    public int Count => _count;

    public clsList()
    {
        arr = new T[_DefaultSize];
        _Size = _DefaultSize;
        _count = 0;
    }

    public clsList(int size)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than 0");

        arr = new T[size];
        _Size = size;
        _count = 0;
    }

    public clsList(IEnumerable<T> collection)
    {
        arr = new T[_DefaultSize];
        _Size = _DefaultSize;
        _count = 0;

        foreach (var item in collection)
            Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
            yield return arr[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return;
        }

        if (_count == _Size)
            Resize();

        arr[_count++] = item;
    }

    private void Resize()
    {
        int newCapacity = arr.Length == 0 ? _DefaultSize : arr.Length * 2;
        _Size = newCapacity;

        T[] newArr = new T[newCapacity];
        Array.Copy(arr, newArr, _count);
        arr = newArr;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return arr[index];
        }
        set
        {
            if (IsReadOnly)
            {
                Console.WriteLine("=== The List Is Read Only ===");
                return;
            }

            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            arr[index] = value;
        }
    }

    public void Insert(int index, T item)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return;
        }

        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_count == _Size)
            Resize();

        for (int i = _count; i > index; i--)
            arr[i] = arr[i - 1];

        arr[index] = item;
        _count++;
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return;
        }

        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (index < 0 || index > _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var items = new List<T>(collection);
        if (items.Count == 0)
            return;

        while (_count + items.Count > _Size)
            Resize();

        for (int j = _count - 1; j >= index; j--)
            arr[j + items.Count] = arr[j];

        for (int i = 0; i < items.Count; i++)
            arr[index + i] = items[i];

        _count += items.Count;
    }

    public void Clear()
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return;
        }

        _count = 0;
        arr = new T[_DefaultSize];
        _Size = _DefaultSize;
    }

    public bool Contains(T item) => IndexOf(item) != -1;

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex + _count > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        for (int i = 0; i < _count; i++)
            array[arrayIndex + i] = arr[i];
    }

    public bool Remove(T item)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return false;
        }

        int index = IndexOf(item);
        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
            if (EqualityComparer<T>.Default.Equals(arr[i], item))
                return i;
        return -1;
    }

    public void RemoveAt(int index)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return;
        }

        if (index < 0 || index >= _count)
            throw new ArgumentOutOfRangeException(nameof(index));

        for (int i = index; i < _count - 1; i++)
            arr[i] = arr[i + 1];

        _count--;
    }

    public int RemoveAll(Predicate<T> match)
    {
        if (IsReadOnly)
        {
            Console.WriteLine("=== The List Is Read Only ===");
            return 0;
        }

        int newIndex = 0;
        int removedCount = 0;

        for (int i = 0; i < _count; i++)
        {
            if (!match(arr[i]))
            {
                arr[newIndex++] = arr[i];
            }
            else
            {
                removedCount++;
            }
        }

        _count = newIndex;
        return removedCount;
    }

    public void ForEach(Action<T> action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        for (int i = 0; i < _Size; i++)
        {
            action(arr[i]);
        }
    }


}

internal class Program
{
    static void Main()
    {
        var numbers = new clsList<int>(5);

        TestAdd(numbers);
        TestInsert(numbers);
        TestInsertRange(numbers);
        TestRemoveAll(numbers);
        TestForEach(numbers);

        Console.ReadKey();
    }

    static void TestAdd(clsList<int> list)
    {
        list.Add(11);
        Console.WriteLine("After adding 11: " + string.Join(", ", list));
    }

    static void TestInsert(clsList<int> list)
    {
        list.Insert(0, 0);
        Console.WriteLine("After inserting 0 at the beginning: " + string.Join(", ", list));
    }

    static void TestInsertRange(clsList<int> list)
    {
        list.InsertRange(list.Count, new clsList<int> { 55, 56 });
        Console.WriteLine("After inserting 55 and 56: " + string.Join(", ", list));
    }

    static void TestRemoveAll(clsList<int> list)
    {
        bool IsEven(int n) => n % 2 == 0;

        int removed = list.RemoveAll(IsEven);
        Console.WriteLine($"Removed {removed} even numbers: " + string.Join(", ", list));
    }

    static void TestForEach(clsList<int> list)
    {
        Console.WriteLine("ForEach Test:");
        list.ForEach(n => Console.WriteLine(n));
    }
}

