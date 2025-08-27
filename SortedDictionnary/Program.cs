using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



public class clsSortedDictionnary<TKey,TValue>: IDictionary<TKey, TValue>
{
    private Node _root;
    private int _count;

    public int Count => _count;

    public bool IsReadOnly => false;

    public bool IsEmpty => _count == 0;
    private readonly IComparer<TKey> _comparer;

    private enum NodeColor { Red, Black }
    private class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Left;
        public Node Right;
        public Node Parent;
        public NodeColor Color;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Left = null;
            Right = null;
            Parent = null;
            Color = NodeColor.Red; // new nodes are red by default

        }
    }

    public clsSortedDictionnary() : this(Comparer<TKey>.Default) { }
    public clsSortedDictionnary(IComparer<TKey> comparer)
    {
        _comparer = comparer ?? Comparer<TKey>.Default;
        _root = null;
        _count = 0;
    }


    public bool Add(TKey key, TValue item)
    {
        Node parent = null;
        Node current = _root;
        int cmp = 0;

        while (current != null)
        {
            cmp = _comparer.Compare(key, current.Key);

            if (cmp == 0)
                return false; // duplicate - SortedSet ignores duplicates
            parent = current;
            if (cmp < 0) current = current.Left;
            else current = current.Right;
        }

        var newNode = new Node(key, item);

        newNode.Parent = parent;

        if (parent == null)
        {
            // tree was empty -> new node becomes root
            _root = newNode;
        }
        else if (_comparer.Compare(key, parent.Key) < 0)
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

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        Add(key, value); // call your custom Add<T> that returns bool
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

    public bool Search(TKey key)
    {
        Node current = _root;
        while (current != null)
        {
            int cmp = _comparer.Compare(key, current.Key);
            if (cmp == 0)
                return true;
            else if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return false;
    }

    public bool Remove(TKey item)
    {
        return true;
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    public bool ContainsKey(TKey key)
    {
        return Search(key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue val))
        {
            return EqualityComparer<TValue>.Default.Equals(val, item.Value);
        }
        return false;
    }

    public bool ContainsValue(TValue value)
    {
        EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
        return ContainsValueRecursive(_root, value, comparer);
    }

    private bool ContainsValueRecursive(Node node, TValue value, EqualityComparer<TValue> comparer)
    {
        if (node == null)
            return false;

        // Check current node
        if (comparer.Equals(node.Value, value))
            return true;

        // Check left and right
        return ContainsValueRecursive(node.Left, value, comparer) ||
               ContainsValueRecursive(node.Right, value, comparer);
    }


    public KeyValuePair<TKey, TValue> Min
    {
        get
        {
            if (_root == null) throw new InvalidOperationException("Dictionary is empty");
            var current = _root;
            while (current.Left != null)
                current = current.Left;
            return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
        }
    }

    public KeyValuePair<TKey, TValue> Max
    {
        get
        {
            if (_root == null) throw new InvalidOperationException("Dictionary is empty");
            var current = _root;
            while (current.Right != null)
                current = current.Right;
            return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal(_root).GetEnumerator();
    }

    private IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(Node node)
    {
        if (node != null)
        {
            foreach (var left in InOrderTraversal(node.Left))
                yield return left;

            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);

            foreach (var right in InOrderTraversal(node.Right))
                yield return right;
        }
    }

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


    public ICollection<TKey> Keys => new KeysCollection(this);
    public ICollection<TValue> Values => new ValuesCollection(this);

    public class KeysCollection : ICollection<TKey>
    {
        private readonly clsSortedDictionnary<TKey, TValue> _dictionary;

        public KeysCollection(clsSortedDictionnary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => true;

        public void CopyTo(TKey[] array, int arrayIndex)
        {
            foreach (var kv in _dictionary)
            {
                array[arrayIndex++] = kv.Key;
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            foreach (var kv in _dictionary)
                yield return kv.Key;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Not supported (read-only view)
        public void Add(TKey item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Remove(TKey item) => throw new NotSupportedException();
        public bool Contains(TKey item) => _dictionary.ContainsKey(item);
    }

    public class ValuesCollection : ICollection<TValue>
    {
        private readonly clsSortedDictionnary<TKey, TValue> _dictionary;

        public ValuesCollection(clsSortedDictionnary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => true;

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            foreach (var kv in _dictionary)
            {
                array[arrayIndex++] = kv.Value;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (var kv in _dictionary)
                yield return kv.Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Not supported
        public void Add(TValue item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Remove(TValue item) => throw new NotSupportedException();
        public bool Contains(TValue item) => _dictionary.ContainsValue(item);
    }


    public TValue this[TKey key]
    {
        get
        {
            Node current = _root;
            while (current != null)
            {
                int cmp = _comparer.Compare(key, current.Key);
                if (cmp == 0)
                    return current.Value;
                current = (cmp < 0) ? current.Left : current.Right;
            }
            throw new KeyNotFoundException();
        }
        set
        {
            Node current = _root;
            while (current != null)
            {
                int cmp = _comparer.Compare(key, current.Key);
                if (cmp == 0)
                {
                    current.Value = value;
                    return;
                }
                current = (cmp < 0) ? current.Left : current.Right;
            }
            // if not found, insert new
            Add(key, value);
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var node = _root;
        while (node != null)
        {
            int cmp = _comparer.Compare(key, node.Key);
            if (cmp == 0)
            {
                value = node.Value;
                return true;
            }
            node = (cmp < 0) ? node.Left : node.Right;
        }

        value = default;
        return false;
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> GetViewBetween(TKey lower, TKey upper)
    {
        if (_comparer.Compare(lower, upper) > 0)
            throw new ArgumentException("lower bound must not be greater than upper bound");

        return GetViewBetween(_root, lower, upper);
    }

    private IEnumerable<KeyValuePair<TKey, TValue>> GetViewBetween(Node node, TKey lower, TKey upper)
    {
        if (node == null) yield break;

        if (_comparer.Compare(node.Key, lower) > 0)
            foreach (var left in GetViewBetween(node.Left, lower, upper))
                yield return left;

        if (_comparer.Compare(node.Key, lower) >= 0 && _comparer.Compare(node.Key, upper) <= 0)
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);

        if (_comparer.Compare(node.Key, upper) < 0)
            foreach (var right in GetViewBetween(node.Right, lower, upper))
                yield return right;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        if (array.Length - arrayIndex < this.Count)
            throw new ArgumentException("The array is too small.");

        foreach (var kvp in this)
        {
            array[arrayIndex++] = kvp;
        }
    }


}


internal class Program
{
    static void Main(string[] args)
    {
        var dict = new clsSortedDictionnary<int, string>();

        // Add items
        dict.Add(10, "ten");
        dict.Add(5, "five");
        dict.Add(20, "twenty");
        dict.Add(15, "fifteen");
        dict.Add(25, "twenty-five");

        Console.WriteLine("Count: " + dict.Count);  // expect 5

        // ContainsKey
        Console.WriteLine("Contains 10? " + dict.ContainsKey(10)); // true
        Console.WriteLine("Contains 99? " + dict.ContainsKey(99)); // false

        // Indexer get/set
        Console.WriteLine("Value for key 15: " + dict[15]); // fifteen
        dict[15] = "updated";
        Console.WriteLine("Updated value for 15: " + dict[15]); // updated

        // Keys and Values
        Console.WriteLine("Keys: " + string.Join(", ", dict.Keys));
        Console.WriteLine("Values: " + string.Join(", ", dict.Values));

        // TryGetValue
        if (dict.TryGetValue(20, out var val))
            Console.WriteLine("TryGetValue(20) = " + val); // twenty

       
        Console.WriteLine("Etirating Keys");
        foreach (var key in dict.Keys)
            Console.WriteLine(key);

        Console.WriteLine("Etirating Values");
        foreach (var value in dict.Values)
            Console.WriteLine(value);

        // Min / Max
        Console.WriteLine("Min = " + dict.Min);
        Console.WriteLine("Max = " + dict.Max);

        // Enumeration
        Console.WriteLine("In-order traversal:");
        foreach (var kv in dict)
            Console.WriteLine($"{kv.Key} -> {kv.Value}");

        // Clear
        dict.Clear();
        Console.WriteLine("Count after Clear: " + dict.Count); // expect 0

        Console.ReadKey();
    }

}


