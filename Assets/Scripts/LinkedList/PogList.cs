using System;

/// <summary>
/// Play Of the Game List
/// This is my version of LinkedList
/// </summary>
/// <typeparam name="T"></typeparam>
public class PogList<T> : IDynamicList<T>
{
    private Node<T> _head;
    private int _count;

    public PogList()
    {
        _count = 0;
        _head = null;
    }

    public int Count => _count;

    public int IndexOf(T item)
    {
        Node<T> currentNode = null;
        int currentIndex = 0;
        int foundIndex = -1;

        if (_head != null)
        {
            do
            {
                currentNode = GetNextNode(currentNode);

                if (currentNode.Value.Equals(item))
                {
                    foundIndex = currentIndex;
                    break;
                }

                currentIndex++;
            } while (currentNode.Next != null);
        }

        return foundIndex;
    }

    public bool Contains(T item)
    {
        Node<T> currentNode = null;
        bool itemFound = false;

        if (_head != null)
        {
            do
            {
                currentNode = GetNextNode(currentNode);

                if (currentNode.Value.Equals(item))
                {
                    itemFound = true;
                    break;
                }
            } while (currentNode.Next != null);
        }

        return itemFound;
    }

    public void Add(T item)
    {
        void AddNext(Node<T> head, T value)
        {
            if (head.Next != null)
            {
                AddNext(head.Next, value);
            }
            else
            {
                head.SetNext(new Node<T>(value));
            }
        }

        if (_head == null)
        {
            _head = new Node<T>(item);
        }
        else
        {
            AddNext(_head, item);
        }

        _count++;
    }

    public void Insert(int index, T item)
    {
        Node<T> currentNode = null;
        Node<T> previousNode = null;
        int currentIndex = 0;

        if (index < _count && _head != null)
        {
            do
            {
                currentNode = GetNextNode(currentNode);

                if (currentIndex == index)
                {
                    if (previousNode != null)
                    {
                        previousNode.SetNext(new Node<T>(item, currentNode));
                    }
                    else
                    {
                        _head = new Node<T>(item, currentNode);
                        currentNode = _head;
                    }

                    _count++;
                    return;
                }

                previousNode = currentNode;
                currentIndex++;
            } while (currentNode.Next != null);
        }

        throw new ArgumentOutOfRangeException();
    }

    public void RemoveAt(int index)
    {
        Node<T> currentNode = null;
        Node<T> previousNoded = null;
        int currentIndex = 0;

        if (_head != null)
        {
            do
            {
                currentNode = GetNextNode(currentNode);

                if (currentIndex == index)
                {
                    if (previousNoded != null)
                    {
                        previousNoded.SetNext(currentNode.Next);
                    }
                    else
                    {
                        _head = currentNode.Next;
                        currentNode = _head;
                    }

                    _count--;
                    return;
                }

                previousNoded = currentNode;
                currentIndex++;
            } while (currentNode.Next != null);
        }

        throw new ArgumentOutOfRangeException();
    }

    public bool Remove(T item)
    {
        Node<T> currentNode = null;
        Node<T> previousNode = null;
        bool isRemoved = false;

        if (_head != null)
        {
            do
            {
                currentNode = GetNextNode(currentNode);

                if (currentNode.Value.Equals(item))
                {
                    if (previousNode != null)
                    {
                        previousNode.SetNext(currentNode.Next);
                    }
                    else
                    {
                        _head = currentNode.Next;
                    }

                    isRemoved = true;
                    _count--;
                }

                previousNode = currentNode;
            } while (currentNode.Next != null);
        }

        return isRemoved;
    }

    public void Clear()
    {
        if (_head != null)
        {
            Node<T> currentNode = _head;
            while (currentNode != null)
            {
                Node<T> tempNode = currentNode;
                currentNode = currentNode.Next;
                tempNode.Clear();
            }
        }

        _head = null;
        _count = 0;
    }

    public void CopyTo(T[] target, int index)
    {
        if (target == null)
        {
            throw new ArgumentNullException();
        }

        if (index < 0 || index > target.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (target.Length - index < _count)
        {
            throw new ArgumentException();
        }

        if (_head == null)
        {
            return;
        }

        Node<T> currentNode = _head;

        do
        {
            target[index++] = currentNode.Value;
            currentNode = currentNode.Next;
        } while (currentNode != null);
    }

    public T this[int index]
    {
        get
        {
            if (_head != null && index < _count)
            {
                Node<T> currentNode = null;
                int currentCount = 0;

                while (currentCount < _count)
                {
                    currentNode = GetNextNode(currentNode);

                    if (currentCount == index)
                    {
                        return currentNode.Value;
                    }

                    currentCount++;
                }
            }

            throw new IndexOutOfRangeException();
        }
        set
        {
            if (_head != null && index < _count)
            {
                Node<T> currentNode = null;
                Node<T> previousNode = null;
                int currentCount = 0;

                while (currentCount < _count)
                {
                    currentNode = GetNextNode(currentNode);

                    if (currentCount == index)
                    {
                        if (previousNode == null)
                        {
                            _head = new Node<T>(value, currentNode.Next);
                            currentNode = _head;
                        }
                        else
                        {
                            previousNode.Next.SetNext(new Node<T>(value, currentNode.Next));
                        }
                    }

                    previousNode = currentNode;
                    currentCount++;
                }
            }

            throw new IndexOutOfRangeException();
        }
    }

    private Node<T> GetNextNode(Node<T> currentHead)
    {
        return currentHead != null ? currentHead.Next : _head;
    }

    private class Node<TN>
    {
        private TN _value;
        private Node<TN> _next;

        public TN Value => _value;
        public Node<TN> Next => _next;

        public Node(TN value)
        {
            _value = value;
            _next = null;
        }

        public Node(TN value, Node<TN> next)
        {
            _value = value;
            _next = next;
        }

        public void SetNext(Node<TN> next)
        {
            _next = next;
        }

        public void Clear()
        {
            _next = null;
        }
    }
}