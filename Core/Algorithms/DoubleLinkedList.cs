using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Markdown2HTML.Core.Algorithms
{
    public class DoubleLinkedList<T> : IEnumerable<T>
    {
        public class Node
        {
            public Node Next;
            public Node Prev;

            public T Value;

            /// <summary>
            /// The container of this node. 
            /// </summary>
            public DoubleLinkedList<T> Container;

            /// <summary>
            /// Only <see cref="DoubleLinkedList{T}"/>can construct the double-linked list node.
            /// </summary>
            /// <param name="container">Reference to the container</param>
            /// <param name="value">node value</param>
            /// <param name="next">next node</param>
            /// <param name="prev">previous node</param>
            public Node(DoubleLinkedList<T> container, T value, Node next, Node prev)
            {
                Next = next;
                Prev = prev;
                Container = container;
                this.Value = value;
            }

            public Node InsertAfter(T value)
            {
                return Container.InsertAfter(this, value);
            }
            public Node InsertBefore(T value)
            {
                return Container.InsertBefore(this, value);
            }
        }

        private Node _head = null;
        private Node _tail = null;

        public Node Head => _head;
        public Node Tail => _tail;


        public Node Append(T content)
        {
            // ... [Tail]         [node]
            //  next -> null   next ->  null
            //  prev -> ...    prev -> [Tail]
            var node = new Node(this, content, null, _tail);

            if (_tail == null)
            {
                //  [Tail/Head] 
                //  next -> null
                //  prev -> null  
                _tail = node;
                _head = _tail;
            }
            else
            {
                // ... [Tail]         [node]
                //  next ->[node]  next ->  null
                //  prev -> ...    prev -> [Tail]
                _tail.Next = node;

                // ... [Tail]  (previously node)       
                //  next -> null
                //  prev -> ...  
                _tail = node;
            }

            return node;
        }
        public Node Prepend(T content)
        {
            //     [node]         [Head]           ...
            //  next -> [Head]   next ->  ...
            //  prev -> null     prev -> null
            var node = new Node(this, content, _head, null);

            if (_head == null)
            {
                //  [Tail/Head] 
                //  next -> null
                //  prev -> null  
                _head = node;
                _tail = _head;
            }
            else
            {
                //     [node]         [Head]           ...
                //  next -> [Head]   next ->  ...
                //  prev -> null     prev -> [node]
                _head.Prev = node;

                //     [Head]         ...
                //  next -> ...   
                //  prev -> null    
                _head = node;
            }

            return node;
        }

        public override string ToString()
        {
            var curr = _head;
            var sb = new StringBuilder();

            while (curr != null)
            {
                sb.Append(curr.Value);
                curr = curr.Next;
            }

            return sb.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var curr = Head;
            while (curr != null)
            {
                yield return curr.Value;
                curr = curr.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RemoveFirst()
        {
            Remove(Head);
            return;
            /* works, but not needed.
            if (_head == null)
            {
                throw new InvalidOperationException("removing from empty list.");
            }

            if (_head == _tail)
            {
                _head = null;
                _tail = null;
            }
            else
            {
                _head = _head.Next;
                if (_head != null)
                {
                    _head.Prev = null;
                }
            }
        */
        }

        public void RemoveLast()
        {
            Remove(Tail);
            return;
            /* works, but not needed.
            if (_tail == null)
            {
                throw new InvalidOperationException("removing from empty list.");
            }

            if (_head == _tail)
            {
                _head = null;
                _tail = null;
            }
            else
            {
                _tail = _tail.Prev;
                if (_tail != null)
                {
                    _tail.Next = null;
                }
            }
            */
        }

        // Untested method
        public void Remove(Node node)
        {
            if (node.Container != this)
            {
                throw new InvalidOperationException("Node is from another container.");
            }

            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            else if (node == _head)
            {
                _head = node.Next;
            }
            else 
            {
                throw new Exception("Sanity check failed.");
            }

            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            else if (node == Tail)
            {
                _tail = node.Prev;
            }
            else
            {
                throw new Exception("Sanity check failed.");
            }

            node.Container = null; // ensure this node cannot be used again.
        }

        public Node InsertBefore(Node relative, T content)
        {
            if (relative.Container != this)
            {
                throw new InvalidOperationException("Node is from another container.");
            }

            if (relative == null)
            {
                throw new ArgumentException("null relative is not allowed");
            }

            var node = new Node(this, content, relative, relative.Prev);

            if (relative == Head)
            {
                _head = node;
            }
            else
            {
                relative.Prev.Next = node;
            }

            relative.Prev = node;

            return node;
        }
        public Node InsertAfter(Node relative, T content)
        {
            if (relative.Container != this)
            {
                throw new InvalidOperationException("Node is from another container.");
            }

            if (relative == null)
            {
                throw new ArgumentException("null relative is not allowed");
            }

            var node = new Node(this, content, relative.Next, relative);

            if (relative == Tail)
            {
                _tail = node;
            }
            else
            {
                relative.Next.Prev = node;
            }

            relative.Next = node;

            return node;

        }

        public bool Empty => Head == null;

        public int Count()
        {
            var curr = Head;
            int count = 0;
            while (curr != null)
            {
                ++count;
                curr = curr.Next;
            }

            return count;
        }
    }
}