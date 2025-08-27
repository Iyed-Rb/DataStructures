using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class clsLinkedList<T> : IEnumerable<T>
{
    private Node head = null;
    private Node tail;

    private int _count = 0;
    public int Count => _count;

    public class Node
    {
        public T Value;
        public Node next;
        public Node previous;

        public Node(T value)
        {
            Value = value;
            next = null;
            previous = null;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node current = head;
        while (current != null)
        {
            yield return current.Value;
            current = current.next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T value) => AddLast(value);

    public void Remove(T value)
    {
        Node current = head;
        while (current != null)
        {
            if (current.Value.Equals(value))
            {
                // Remove head
                if (current.previous == null)
                    head = current.next;

                // Update previous node
                if (current.previous != null)
                    current.previous.next = current.next;

                // Update next node
                if (current.next != null)
                    current.next.previous = current.previous;

                if (current.next == null) // removed last node
                    tail = current.previous;

                _count--;
                return; // remove only first occurrence
            }

            current = current.next;
        }

    }

    public void RemoveAll(T value)
    {
        Node current = head;
        while (current != null)
        {
            Node nextNode = current.next; // store next because current may be removed

            if (current.Value.Equals(value))
            {
                // Remove head
                if (current.previous == null)
                    head = current.next;

                // Update previous node
                if (current.previous != null)
                    current.previous.next = current.next;

                // Update next node
                if (current.next != null)
                    current.next.previous = current.previous;

                if (current.next == null) // removed last node
                    tail = current.previous;
                _count--;
            }

            current = nextNode; // move forward safely
        }
    }

    public void RemoveFirst()
    {
        if (head == null) return;

        head = head.next;
        if (head != null)
            head.previous = null;

        if (head == null)
            tail = null; // list became empty

        _count--;

    }


    public void RemoveLast()
    {
        if (tail == null) return;

        if (tail.previous != null)
            tail.previous.next = null;
        else
            head = null; // list had only one node

        tail = tail.previous;

        _count--;
    }

    public void AddFirst(T value)
    {
        Node newNode = new Node(value);

        if (head == null)
        {
            head = tail = newNode;
            return;
        }

        newNode.next = head;
        head.previous = newNode;
        head = newNode;

        _count++;
    }

    public void AddLast(T value)
    {
        Node newNode = new Node(value);

        if (head == null)
        {
            head = tail = newNode;
            return;
        }

        tail.next = newNode;
        newNode.previous = tail;
        tail = newNode;

        _count++;
    }

    public bool Contains(T value)
    {
        Node current = head;
        while (current != null)
        {
            if (current.Value.Equals(value))
                return true;
            current = current.next;
        }
        return false;
    }

    public Node Find(T value)
    {
        Node current = head;
        while (current != null)
        {
            if (current.Value.Equals(value))
                return current;
            current = current.next;
        }
        return null;
    }

    public void PrintAll()
    {
        Node current = head;
        while (current != null)
        {
            Console.Write(current.Value + " <-> ");
            current = current.next;
        }
        Console.WriteLine("null");
    }

    public void Clear()
    {
        head = null;
        tail = null;
        // garbage collector handles the rest
        _count = 0;
    }

    public void AddAfter(Node node, T value)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        Node newNode = new Node(value);

        newNode.previous = node;
        newNode.next = node.next;

        if (node.next != null)
            node.next.previous = newNode;
        else
            tail = newNode; // update tail if adding at the end

        node.next = newNode;

        _count++;
    }


    public void AddBefore(Node node, T value)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        Node newNode = new Node(value);

        newNode.next = node;
        newNode.previous = node.previous;

        if (node.previous != null)
            node.previous.next = newNode;
        else
            head = newNode; // update head if adding at the start

        node.previous = newNode;

        _count++;
    }

    public void Remove(Node node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        // Update previous node
        if (node.previous != null)
            node.previous.next = node.next;
        else
            head = node.next; // node was head

        // Update next node
        if (node.next != null)
            node.next.previous = node.previous;
        else
            tail = node.previous; // node was tail

        // Optional: clean the node
        node.next = null;
        node.previous = null;

        _count--;
    }

}
internal class Program
{
    static void Main(string[] args)
    {

        clsLinkedList<int> list = new clsLinkedList<int>();

        // Add nodes
        list.Add(10);
        list.AddFirst(5);
        list.AddLast(20);
        list.AddAfter(list.Find(10), 15);  // 10 -> 15
        list.AddBefore(list.Find(20), 18); // 18 before 20

        Console.WriteLine("List after adding:");
        list.PrintAll(); // Expected: 5 <-> 10 <-> 15 <-> 18 <-> 20 <-> null

        // Remove operations
        list.Remove(15);
        list.RemoveFirst();
        list.RemoveLast();

        Console.WriteLine("List after removing:");
        list.PrintAll(); // Expected: 10 <-> 18 <-> null

        // Remove by node
        var node18 = list.Find(18);
        list.Remove(node18);

        Console.WriteLine("List after removing node 18:");
        list.PrintAll(); // Expected: 10 <-> null

        // Contains / Find
        Console.WriteLine(list.Contains(10)); // True
        Console.WriteLine(list.Contains(100)); // False

        // Clear
        list.Clear();
        Console.WriteLine("List after clear:");
        list.PrintAll(); // null


        Console.ReadKey();
    }
}

