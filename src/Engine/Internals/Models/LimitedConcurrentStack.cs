using System.Collections.Concurrent;

namespace DrawnUi.Maui.Infrastructure.Models;

public class LimitedQueue<T>
{
    private Queue<T> queue = new();
    private readonly int _maxLength = 3;
    private bool _locked;

    public int Count => queue.Count;

    public T[] ToArray()
    {
        return queue.ToArray();
    }

    public List<T> ToList()
    {
        return queue.ToArray().ToList();
    }

    public LimitedQueue()
    {
    }

    protected virtual void OnAutoRemovingItem(T item)
    {

    }

    public LimitedQueue(int max)
    {
        _maxLength = max;
    }

    public void Push(T item)
    {
        if (_locked)
            return;

        queue.Enqueue(item);
        while (queue.Count > _maxLength)
        {
            queue.TryDequeue(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }

    public bool IsLocked
    {
        get
        {
            return _locked;
        }
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public T Pop()
    {
        T latestItem;
        queue.TryDequeue(out latestItem);
        return latestItem;
    }

    public void Clear()
    {
        while (queue.Count > 0)
        {
            queue.TryDequeue(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }
}

public class LimitedConcurrentQueue<T>
{
    private ConcurrentQueue<T> queue = new();
    private readonly int _maxLength = 3;
    private bool _locked;

    public int Count => queue.Count;

    public T[] ToArray()
    {
        return queue.ToArray();
    }

    public List<T> ToList()
    {
        return queue.ToArray().ToList();
    }

    public LimitedConcurrentQueue()
    {
    }

    protected virtual void OnAutoRemovingItem(T item)
    {

    }

    public LimitedConcurrentQueue(int max)
    {
        _maxLength = max;
    }

    public void Push(T item)
    {
        if (_locked)
            return;

        queue.Enqueue(item);
        while (queue.Count > _maxLength)
        {
            queue.TryDequeue(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }

    public bool IsLocked
    {
        get
        {
            return _locked;
        }
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public T Pop()
    {
        T latestItem;
        queue.TryDequeue(out latestItem);
        return latestItem;
    }

    public void Clear()
    {
        while (queue.Count > 0)
        {
            queue.TryDequeue(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }
}

public class LimitedStack<T>
{
    private Stack<T> stack = new();
    private readonly int _maxLength = 3;
    private bool _locked;

    public int Count => stack.Count;

    public T[] ToArray()
    {
        return stack.ToArray();
    }

    public List<T> ToList()
    {
        return stack.ToArray().ToList();
    }

    public LimitedStack()
    {
    }

    protected virtual void OnAutoRemovingItem(T item)
    {

    }

    public LimitedStack(int max)
    {
        _maxLength = max;
    }

    public void Push(T item)
    {
        if (_locked)
            return;

        stack.Push(item);
        while (stack.Count > _maxLength)
        {
            stack.TryPop(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }

    public bool IsLocked
    {
        get
        {
            return _locked;
        }
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public T Pop()
    {
        T latestItem;
        stack.TryPop(out latestItem);
        return latestItem;
    }

    public void Clear()
    {
        while (stack.Count > 0)
        {
            stack.TryPop(out var removedItem);
            OnAutoRemovingItem(removedItem);
        }
    }
}