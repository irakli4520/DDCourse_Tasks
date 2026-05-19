using System.Collections;

namespace MyLinkedList;

public class MyLinkedList<T> : ICollection<T>
{
    public MyNode<T>? First { get; private set; }
    public MyNode<T>? Last { get; private set; }

    public MyNode<T> AddFirst(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        MyNode<T> node = new(value);
        AddFirst(node);
        return node;
    }

    public void AddFirst(MyNode<T> node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        if (First == null)
        {
            Last = node;
        }
        
        node.Next = First;
        if (node.Next != null)
        {
            node.Next.Previous = node;
        }
        First = node;
        Count++;
    }

    public MyNode<T> AddLast(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        MyNode<T> node = new(value);
        AddLast(node);
        return node;
    }

    public void AddLast(MyNode<T> node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        if (Last == null)
        {
            First = node;
        }
        else
        {
            node.Previous = Last;
            Last.Next = node;
        }
        Last = node;
        Count++;
    }

    public MyNode<T> AddAfter(MyNode<T> node, T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        MyNode<T> newNode = new MyNode<T>(value);
        AddAfter(node, newNode);
        return newNode;
    }

    public void AddAfter(MyNode<T> node, MyNode<T> newNode)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        ArgumentNullException.ThrowIfNull(newNode, nameof(newNode));

        newNode.Previous = node;
        if (node.Next != null)
        {
            node.Next.Previous = newNode;
        }
        else
        {
            Last = newNode;
        }
        newNode.Next = node.Next;
        node.Next = newNode;
        Count++;
    }

    public MyNode<T> AddBefore(MyNode<T> node, T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        MyNode<T> newNode = new MyNode<T>(value);
        AddBefore(node, newNode);
        return newNode;
    }

    public void AddBefore(MyNode<T> node, MyNode<T> newNode)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        ArgumentNullException.ThrowIfNull(newNode, nameof(newNode));
        
        newNode.Previous = node.Previous;
        if (node.Previous != null)
        {
            node.Previous.Next = newNode;
        }
        else
        {
            First = newNode;
        }
        node.Previous = newNode;
        newNode.Next = node;
        Count++;
    }

    public MyNode<T>? Find(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        MyNode<T>? current = First;
        while (current != null)
        {
            if (value.Equals(current.Value))
            {
                return current;
            }
            current = current.Next;
        }
        
        return null;
    }

    public MyNode<T>? FindLast(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        MyNode<T>? findLast = null;
        MyNode<T>? current = First;
        while (current != null)
        {
            if (value.Equals(current.Value))
            {
                findLast = current;
            }
            current = current.Next;
        }

        return findLast;
    }

    public void Clear()
    {
        First = null;
        Last = null;
        Count = 0;
    }

    public bool Contains(T value)
    {
        if (Find(value) != null)
        {
            return true;
        }

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        // if (array == null) throw new ArgumentNullException(nameof(array));
        // if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index must be non-negative.");
        // if (array.Length - arrayIndex < Count) throw new ArgumentException("Not enough space in target array.");
        //
        // MyNode<T>? current = First;
        // int i = arrayIndex;
        // while (current != null)
        // {
        //     array[i++] = current.Value;
        //     current = current.Next;
        // }
    }

     public bool Remove(T value)
     {
         ArgumentNullException.ThrowIfNull(value, nameof(value));
         
         MyNode<T>? current = Find(value);
         if (current != null)
         {
             if (current.Next != null)
             {
                 current.Next.Previous = current.Previous;
             }
             else
             {
                 Last = current.Previous;
             }
         
             if (current.Previous != null)
             {
                 current.Previous.Next = current.Next;
             }
             else
             {
                 First = current.Next;
             }
             Count--;
             return true;
         }

         return false;

     }

    public IEnumerator<T> GetEnumerator()
    {
        return new MyLinkedListEnumerator<T>(First);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<T>.Add(T value)
    {
        AddLast(value);
    }

    public int Count { get; private set; }
    bool ICollection<T>.IsReadOnly => false;
}

public class MyLinkedListEnumerator<T> : IEnumerator<T>
{
    private MyNode<T>? _current;
    private readonly MyNode<T>? _first;

    public MyLinkedListEnumerator(MyNode<T>? first)
    {
        _first = first;
        _current = null;
    }
    public bool MoveNext()
    {
        if (_current == null)
        {
            _current = _first;
        }
        else
        {
            _current = _current.Next;
        }

        return _current != null;
    }

    public void Reset()
    {
        _current = null;
    }

    public T Current
    {
        get
        {
            if (_current == null)
                throw new InvalidOperationException(
                    "Enumerator is positioned before the first element or after the last element.");
            return _current.Value;
        }
    }

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
    }
}