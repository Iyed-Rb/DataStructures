using System;
using System.Collections.Generic;

namespace BinarySearchTreeDemo
{

    /*  
     
        public class Node<T> where T : IComparable<T>
          It does not mean we are inheriting the interface.
        It means:
          "I am telling the compiler that T must be a type that implements IComparable<T>."

        It’s a generic constraint.  
        It’s not inheritance. It’s a requirement for the generic type.
     
     
     */
    public class BinarySearchTreeNode<T> where T : IComparable<T>
    {
        public T Value { get; set; }
        public BinarySearchTreeNode<T> Left { get; set; }
        public BinarySearchTreeNode<T> Right { get; set; }

        public BinarySearchTreeNode(T value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }

    public class BinarySearchTree<T> where T : IComparable<T>
    {
        public BinarySearchTreeNode<T> Root { get; private set; }

        public BinarySearchTree()
        {
            Root = null;
        }

        public bool Search(T value)
        {
            return Search(Root, value) != null;
        }

        private BinarySearchTreeNode<T> Search(BinarySearchTreeNode<T> node, T value)
        {
            if (node == null || node.Value.Equals(value))
            {
                return node;
            }
            if (value.CompareTo(node.Value) < 0)
            {
                return Search(node.Left, value);
            }
            else
            {
                return Search(node.Right, value);
            }
        }

        public bool Search2(T Value)
        {
            if (Root == null)
                throw new InvalidOperationException("The Tree is empty");
            if (Value.CompareTo(Root.Value) == 0)
                return true;

            BinarySearchTreeNode<T> Current = Root;
            while (Current != null)
            {
                if (Value.CompareTo(Current.Value) > 0)
                {
                    Current = Current.Right;
                }

                else if (Value.CompareTo(Current.Value) < 0)
                {
                    Current = Current.Left;
                }

                else
                    return true;
            }

            return false;
        }


        public void Insert(T value)
        {
            Root = Insert(Root, value);
        }

        private BinarySearchTreeNode<T> Insert(BinarySearchTreeNode<T> node, T value)
        {
            if (node == null)
            {
                return new BinarySearchTreeNode<T>(value);
            }
            else if (value.CompareTo(node.Value) < 0)
            {
                node.Left = Insert(node.Left, value);
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                node.Right = Insert(node.Right, value);
            }

            return node;
        }

        public void InsertWithoutRecursion(T Value)
        {

            if (Root == null)
            {
                this.Root = new BinarySearchTreeNode<T>(Value);
                return;
            }

            BinarySearchTreeNode<T> Prev = null;
            BinarySearchTreeNode<T> Curr = Root;

            while (Curr != null)
            {
                if (Value.CompareTo(Curr.Value) < 0)
                {
                    Prev = Curr;
                    Curr = Curr.Left;
                }
                else if (Value.CompareTo(Curr.Value) > 0)
                {
                    Prev = Curr;
                    Curr = Curr.Right;
                }
            }
            if (Value.CompareTo(Prev.Value) < 0)
                Prev.Left = new BinarySearchTreeNode<T>(Value);
            else if (Value.CompareTo(Prev.Value) > 0)
                Prev.Right = new BinarySearchTreeNode<T>(Value);
        }

        public void Add(T value)
        {
            var current = Root;

            if (Root == null)
            {
                Root = new BinarySearchTreeNode<T>(value);
                return;
            }

            while (true)
            {
                if (Comparer<T>.Default.Compare(current.Value, value) > 0)
                {
                    if (current.Left == null)
                    {
                        current.Left = new BinarySearchTreeNode<T>(value);
                        break;
                    }
                    else
                        current = current.Left;
                }
                else
                {
                    if (current.Right == null)
                    {
                        current.Right = new BinarySearchTreeNode<T>(value);
                        break;
                    }
                    else
                        current = current.Right;
                }
            }
        }

        // *** Note: Using a Queue adds overhead (extra memory and operations).
        // Normally, you only need a single pointer and recursion/loop.

        //BFS here is unnecessary — you’ll eventually reach the right spot,
        //but only one path matters in BST. BFS makes you explore many irrelevant nodes.
        public void InsertWithQueue(T value)
        {
            var newNode = new BinarySearchTreeNode<T>(value);
            if (Root == null)
            {
                Root = newNode;
                return;
            }

            Queue<BinarySearchTreeNode<T>> queue = new Queue<BinarySearchTreeNode<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (value.CompareTo(current.Value) < 0)
                {
                    if (current.Left == null)
                    {
                        current.Left = newNode;
                        break;
                    }
                    else
                    {
                        queue.Enqueue(current.Left);
                    }
                }
                else
                {
                    if (current.Right == null)
                    {
                        current.Right = newNode;
                        break;
                    }
                    else
                    {
                        queue.Enqueue(current.Right);
                    }
                }
            }
        }

        public void InOrderTraversal()
        {
            InOrderTraversal(Root);
            Console.WriteLine();
        }

        private void InOrderTraversal(BinarySearchTreeNode<T> node)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left);
                Console.Write(node.Value + " ");
                InOrderTraversal(node.Right);
            }
        }

        public void PreOrderTraversal()
        {
            PreOrderTraversal(Root);
            Console.WriteLine();
        }

        private void PreOrderTraversal(BinarySearchTreeNode<T> node)
        {
            if (node != null)
            {
                Console.Write(node.Value + " ");
                PreOrderTraversal(node.Left);
                PreOrderTraversal(node.Right);
            }
        }

        public void PostOrderTraversal()
        {
            PostOrderTraversal(Root);
            Console.WriteLine();
        }

        private void PostOrderTraversal(BinarySearchTreeNode<T> node)
        {
            if (node != null)
            {
                PostOrderTraversal(node.Left);
                PostOrderTraversal(node.Right);
                Console.Write(node.Value + " ");
            }
        }

        // Print the tree visually
        public void PrintTree()
        {
            PrintTree(Root, 0);
        }


        private void PrintTree(BinarySearchTreeNode<T> root, int space)
        {
            int COUNT = 10;  // Distance between levels
            if (root == null)
                return;

            space += COUNT;
            PrintTree(root.Right, space);

            Console.WriteLine();
            for (int i = COUNT; i < space; i++)
                Console.Write(" ");
            Console.WriteLine(root.Value);
            PrintTree(root.Left, space);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nInserting : 45, 15, 79, 90, 10, 55, 12, 20, 50\r\n");

            var bst = new BinarySearchTree<int>();
            bst.Insert(45);
            bst.Insert(15);
            bst.Insert(79);
            bst.Insert(90);
            bst.Insert(10);
            bst.Insert(55);
            bst.Insert(12);
            bst.Insert(20);
            bst.Insert(50);
            bst.PrintTree();

            Console.WriteLine("\nInOrder Traversal:");
            bst.InOrderTraversal();

            Console.WriteLine("\nPreOrder Traversal:");
            bst.PreOrderTraversal();

            Console.WriteLine("\nPostOrder Traversal:");
            bst.PostOrderTraversal();

            Console.ReadKey();

        }
    }
}