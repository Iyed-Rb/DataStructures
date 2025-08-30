using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*     

 Well, Stack Colleciton in the .Net use internally a an array and it do the resizing when it needs
 me i used List directly bcz i already built the List Collection before
 
 */

class clsStack<T> : IEnumerable<T>
{
    private List<T> _items = new List<T>();

    public int Count => _items.Count;

    public void Push(T item)
    {
        _items.Add(item);
    }

    public T Pop()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        T value = _items[_items.Count - 1];
        _items.RemoveAt(_items.Count - 1);
        return value;
    }

    public T Peek()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        return _items[_items.Count - 1];
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public T[] ToArray()
    {
        // Important: Stack returns reversed order (top of stack = index 0)
        T[] arr = _items.ToArray();
        Array.Reverse(arr);
        return arr;
    }

    // Implementation of IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        // Enumerate from top to bottom
        for (int i = _items.Count - 1; i >= 0; i--)
            yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class Program
{
    static void Main(string[] args)
    {

        var stack = new clsStack<int>();

        Console.WriteLine("Pushing items: 10, 20, 30");
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);

        Console.WriteLine($"Count: {stack.Count}"); // 3

        Console.WriteLine($"Peek: {stack.Peek()}"); // 30

        Console.WriteLine($"Pop: {stack.Pop()}");   // 30
        Console.WriteLine($"Pop: {stack.Pop()}");   // 20

        Console.WriteLine($"Contains 10? {stack.Contains(10)}"); // true
        Console.WriteLine($"Contains 20? {stack.Contains(20)}"); // false

        Console.WriteLine($"Count after pops: {stack.Count}"); // 1

        Console.WriteLine("Iterating over stack:");
        foreach (var item in stack)
            Console.WriteLine(item); // Should print 10

        Console.WriteLine("Converting to array:");
        int[] arr = stack.ToArray();
        Console.WriteLine(string.Join(", ", arr)); // Should print "10"

        stack.Clear();
        Console.WriteLine($"Count after Clear: {stack.Count}"); // 0

        Console.ReadKey();
    }
}

