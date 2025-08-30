using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class clsQueue<T> : IEnumerable<T>
{
    private T[] _items;
    private int _head;   // the index of the element to be removed next (the front of the queue).
    private int _tail;   // the index of the position where the next new element will be added (the back of the queue).
    private int _size;   // number of elements
    private const int DefaultCapacity = 4;

    public clsQueue()
    {
        _items = new T[DefaultCapacity];
    }

    public int Count => _size;

    public void Enqueue(T item)
    {
        if (_size == _items.Length)
        {
            Resize();
        }

        _items[_tail] = item;
        _tail = (_tail + 1) % _items.Length; // circular move
        _size++;

        // _items.Length: is the capacity of the array (the total number of slots it can hold).
        // Important: this includes all slots (whether they are filled or empty).
    }

    public T Dequeue()
    {
        if (_size == 0)
            throw new InvalidOperationException("Queue is empty.");

        T value = _items[_head];
        _items[_head] = default; // optional: clear for GC
        _head = (_head + 1) % _items.Length; // circular move
        _size--;

        return value;
    }

    public T Peek()
    {
        if (_size == 0)
            throw new InvalidOperationException("Queue is empty.");
        return _items[_head];
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _items.Length); // optional, clear for GC
        _head = 0;
        _tail = 0;
        _size = 0;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < _size; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[(_head + i) % _items.Length], item))
                return true;
        }
        return false;
    }
    private void Resize()
    {
        int newCapacity = _items.Length * 2;
        T[] newArray = new T[newCapacity];

        // Copy existing queue elements in order
        if (_head < _tail)
        {
            Array.Copy(_items, _head, newArray, 0, _size);
        }
        else
        {
            Array.Copy(_items, _head, newArray, 0, _items.Length - _head);
            Array.Copy(_items, 0, newArray, _items.Length - _head, _tail);
        }

        _items = newArray;
        _head = 0;
        _tail = _size;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _size; i++)
            yield return _items[(_head + i) % _items.Length];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class Program
{
    static void Main(string[] args)
    {
        clsQueue<int> queue = new clsQueue<int>();

        Console.WriteLine("Enqueue 10, 20, 30");
        queue.Enqueue(10);
        queue.Enqueue(20);
        queue.Enqueue(30);

        Console.WriteLine($"Peek: {queue.Peek()}"); // 10

        Console.WriteLine("Dequeue: " + queue.Dequeue()); // 10
        Console.WriteLine("Dequeue: " + queue.Dequeue()); // 20

        Console.WriteLine("Enqueue 40, 50, 60");
        queue.Enqueue(40);
        queue.Enqueue(50);
        queue.Enqueue(60); // triggers wrap-around

        Console.WriteLine("Queue contains 50? " + queue.Contains(50)); // True
        Console.WriteLine("Queue contains 10? " + queue.Contains(10)); // False

        Console.WriteLine("Queue elements in order:");
        foreach (var item in queue)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Clear queue");
        queue.Clear();
        Console.WriteLine("Count after clear: " + queue.Count); // 0

        // Try enqueue/dequeue again
        queue.Enqueue(70);
        Console.WriteLine("Peek after enqueue 70: " + queue.Peek()); // 70
    }
}

