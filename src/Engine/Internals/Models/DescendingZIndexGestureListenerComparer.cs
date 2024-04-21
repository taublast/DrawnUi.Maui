using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Draw;

public class SortedGestureListeners
{
    protected readonly Dictionary<Guid, ISkiaGestureListener> _dic = new();
    protected List<ISkiaGestureListener> Sorted { get; set; }
    protected bool _isDirty = true;

    public List<ISkiaGestureListener> GetListeners()
    {
        lock (_dic)
        {
            if (_isDirty)
            {
                Sorted = _dic.Values
                    .OrderByDescending(listener => listener.ZIndex)
                    .ThenByDescending(listener => listener.GestureListenerRegistrationTime)
                    .ToList();
                _isDirty = false;
            }
            return Sorted;
        }
    }

    public void Add(ISkiaGestureListener item)
    {
        lock (_dic)
        {
            _dic[item.Uid] = item;
            _isDirty = true;
        }
    }

    public void Remove(ISkiaGestureListener item)
    {
        lock (_dic)
        {
            if (_dic.Remove(item.Uid))
            {
                _isDirty = true;
            }
        }
    }

    public void Clear()
    {
        lock (_dic)
        {
            _dic.Clear();
            _isDirty = true;
        }
    }

    public void Invalidate()
    {
        lock (_dic)
        {
            _isDirty = true;
        }
    }
}

public class ControlsTracker
{
    protected readonly Dictionary<Guid, SkiaControl> _dic = new();
    protected List<SkiaControl> Sorted { get; set; }
    protected bool _isDirty = true;

    public void Clear()
    {
        lock (_dic)
        {
            _dic.Clear();
            _isDirty = true;
        }
    }

    public List<SkiaControl> GetList()
    {
        lock (_dic)
        {
            if (_isDirty)
            {
                Sorted = _dic.Values
                    .ToList();
                _isDirty = false;
            }
            return Sorted;
        }
    }

    public void Add(SkiaControl item)
    {
        lock (_dic)
        {
            _dic[item.Uid] = item;
            _isDirty = true;
        }
    }

    public void Remove(SkiaControl item)
    {
        lock (_dic)
        {
            if (_dic.Remove(item.Uid))
            {
                _isDirty = true;
            }
        }
    }

    public void Invalidate()
    {
        lock (_dic)
        {
            _isDirty = true;
        }
    }
}

public class DescendingZIndexGestureListenerComparer : IComparer<ISkiaGestureListener>
{
    public int Compare(ISkiaGestureListener x, ISkiaGestureListener y)
    {
        // Compare y to x instead of x to y to sort in descending order
        int result = y.ZIndex.CompareTo(x.ZIndex);

        // If ZIndex are equal, compare RegistrationTime in descending order
        if (result == 0)
        {
            result = x.GestureListenerRegistrationTime.CompareTo(y.GestureListenerRegistrationTime);
        }

        // If RegistrationTime is equal, compare Uid to ensure uniqueness
        if (result == 0)
        {
            result = y.Uid.CompareTo(x.Uid);
        }

        return result;
    }
}