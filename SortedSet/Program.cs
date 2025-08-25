using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


class clsSortedSet <T>: IEnumerable<T>, ICollection<T>
{
    private Node _root;
    private int _count;

    public int Count => _count;

    public bool IsReadOnly => false;

    public bool IsEmpty => _count == 0;
    private readonly IComparer<T> _comparer;
    private enum NodeColor { Red, Black }

    public clsSortedSet() : this(null) { }
    public clsSortedSet(IComparer<T> comparer)
    {
        _comparer = comparer ?? Comparer<T>.Default;
        _root = null;
        _count = 0;
    }
    private class Node
    {
        public T Value;
        public Node Left;
        public Node Right;
        public Node Parent;
        public NodeColor Color;


        public Node(T value)
        {
            Value = value;
            Left = null;
            Right = null;
            Parent = null;
            Color = NodeColor.Red; // new nodes are red by default

        }
    }

    public bool Add(T item)
    {
        Node parent = null;
        Node current = _root;
        int cmp = 0;

        while (current != null)
        {
            cmp = _comparer.Compare(item, current.Value);
            if (cmp == 0)
                return false; // duplicate - SortedSet ignores duplicates
            parent = current;
            if (cmp < 0) current = current.Left;
            else current = current.Right;
        }

        var newNode = new Node(item);
        newNode.Parent = parent;

        if (parent == null)
        {
            // tree was empty -> new node becomes root
            _root = newNode;
        }
        else if (_comparer.Compare(item, parent.Value) < 0)
        {
            parent.Left = newNode;
        }
        else
        {
            parent.Right = newNode;
        }

        _count++;               
        FixInsert(newNode);       // restore red-black properties
        _root.Color = NodeColor.Black; // root must be black
        return true;
    }

    void ICollection<T>.Add(T item)
    {
        Add(item); // call your custom Add<T> that returns bool
    }

    private void FixInsert(Node z)
    {
        // While parent exists and is red (violation of red-black property)
        while (z.Parent != null && z.Parent.Color == NodeColor.Red)
        {
            // Parent is left child of grandparent
            if (z.Parent == z.Parent.Parent.Left)
            {
                Node y = z.Parent.Parent.Right; // uncle

                // Case 1: Uncle is red → recolor
                if (y != null && y.Color == NodeColor.Red)
                {
                    z.Parent.Color = NodeColor.Black;
                    y.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    z = z.Parent.Parent; // move z up
                }
                else
                {
                    // Case 2: z is right child → rotate left
                    if (z == z.Parent.Right)
                    {
                        z = z.Parent;
                        RotateLeft(z);
                    }

                    // Case 3: z is left child → rotate right
                    z.Parent.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    RotateRight(z.Parent.Parent);
                }
            }
            else
            {
                // Parent is right child of grandparent (mirror case)
                Node y = z.Parent.Parent.Left; // uncle

                // Case 1 mirror: uncle is red
                if (y != null && y.Color == NodeColor.Red)
                {
                    z.Parent.Color = NodeColor.Black;
                    y.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    z = z.Parent.Parent; // move z up
                }
                else
                {
                    // Case 2 mirror: z is left child
                    if (z == z.Parent.Left)
                    {
                        z = z.Parent;
                        RotateRight(z);
                    }

                    // Case 3 mirror: z is right child
                    z.Parent.Color = NodeColor.Black;
                    z.Parent.Parent.Color = NodeColor.Red;
                    RotateLeft(z.Parent.Parent);
                }
            }
        }

        // Root must always be black
        _root.Color = NodeColor.Black;
    }

    private void RotateLeft(Node x)
    {
        Node y = x.Right;
        x.Right = y.Left;
        if (y.Left != null)
            y.Left.Parent = x;

        y.Parent = x.Parent;
        if (x.Parent == null)
            _root = y;
        else if (x == x.Parent.Left)
            x.Parent.Left = y;
        else
            x.Parent.Right = y;

        y.Left = x;
        x.Parent = y;
    }

    private void RotateRight(Node x)
    {
        Node y = x.Left;
        x.Left = y.Right;
        if (y.Right != null)
            y.Right.Parent = x;

        y.Parent = x.Parent;
        if (x.Parent == null)
            _root = y;
        else if (x == x.Parent.Right)
            x.Parent.Right = y;
        else
            x.Parent.Left = y;

        y.Right = x;
        x.Parent = y;
    }

    public bool Search(T value)
    {
        Node current = _root;
        while (current != null)
        {
            int cmp = _comparer.Compare(value, current.Value);
            if (cmp == 0)
                return true;
            else if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return false;
    }

    public bool Remove(T item)
    {
        return true;
    }


    public bool Contains(T item)
    {
        Node current = _root;
        while (current != null)
        {
            int cmp = _comparer.Compare(item, current.Value);
            if (cmp == 0)
                return true;
            else if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return false;
    }

    public T Min
    {
        get
        {
            if (_root == null) throw new InvalidOperationException("Set is empty");
            var current = _root;
            while (current.Left != null)
                current = current.Left;
            return current.Value;
        }
    }

    public T Max
    {
        get
        {
            if (_root == null) throw new InvalidOperationException("Set is empty");
            var current = _root;
            while (current.Right != null)
                current = current.Right;
            return current.Value;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal(_root).GetEnumerator();
    }

    private IEnumerable<T> InOrderTraversal(Node node)
    {
        if (node != null)
        {
            foreach (var left in InOrderTraversal(node.Left))
                yield return left;

            yield return node.Value;

            foreach (var right in InOrderTraversal(node.Right))
                yield return right;
        }
    }

    // Required for non-generic IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public void Clear()
    {
        // Or Nodes Are Linked Together
        //by doing _root = null, we break the external reference 
        //no outside code can reach those nodes anymore
        // the garbage collector will reclaim that memory

        _root = null;
        _count = 0;
    }

    public bool TryGetValue(T equalValue, out T actualValue)
    {
        var node = _root;
        while (node != null)
        {
            int cmp = _comparer.Compare(equalValue, node.Value);
            if (cmp == 0)
            {
                actualValue = node.Value;
                return true;
            }
            node = (cmp < 0) ? node.Left : node.Right;
        }

        actualValue = default; // default value of T (e.g. null for classes, 0 for ints, etc.).

        return false;
    }

    public IEnumerable<T> GetViewBetween(T lower, T upper)
    {
        if (_comparer.Compare(lower, upper) > 0)
            throw new ArgumentException("lower bound must not be greater than upper bound");

        return GetViewBetween(_root, lower, upper);
    }

    private IEnumerable<T> GetViewBetween(Node node, T lower, T upper)
    {
        if (node == null) yield break;

        if (_comparer.Compare(node.Value, lower) > 0)
            foreach (var left in GetViewBetween(node.Left, lower, upper))
                yield return left;

        if (_comparer.Compare(node.Value, lower) >= 0 && _comparer.Compare(node.Value, upper) <= 0)
            yield return node.Value;

        if (_comparer.Compare(node.Value, upper) < 0)
            foreach (var right in GetViewBetween(node.Right, lower, upper))
                yield return right;
    }

    public void CopyTo(T[] array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (index < 0 || index > array.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (array.Length - index < _count) throw new ArgumentException("Destination array is too small.");

        foreach (var item in this)
        {
            array[index++] = item;
        }
    }

}


internal class Program
{
    static void Main(string[] args)
    {

        var sortedSet = new clsSortedSet<int>();
        sortedSet.Add(5);
        sortedSet.Add(3);
        sortedSet.Add(8);
        sortedSet.Add(1);
        sortedSet.Add(4);

        Console.WriteLine("SortedSet contains:");
        foreach (var item in sortedSet)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine($"Min: {sortedSet.Min}");
        Console.WriteLine($"Max: {sortedSet.Max}");

        int searchValue = 3;
        Console.WriteLine($"Contains {searchValue}: {sortedSet.Contains(searchValue)}");

        int tryGetValue = 4;
        if (sortedSet.TryGetValue(tryGetValue, out int actualValue))
        {
            Console.WriteLine($"Found value: {actualValue}");
        }
        else
        {
            Console.WriteLine($"{tryGetValue} not found.");
        }

        Console.WriteLine("View between 2 and 5:");
        foreach (var item in sortedSet.GetViewBetween(2, 5))
        {
            Console.WriteLine(item);
        }

        int[] array = new int[10];
        sortedSet.CopyTo(array, 2);
        Console.WriteLine("Array after CopyTo:");
        Console.WriteLine(string.Join(", ", array));

        Console.ReadKey();
    }

}

