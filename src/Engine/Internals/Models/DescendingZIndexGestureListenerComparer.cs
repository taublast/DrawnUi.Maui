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

public class LayoutStructure : DynamicGrid<ControlInStack>
{
    public LayoutStructure()
    {

    }

    public LayoutStructure(List<List<ControlInStack>> grid)
    {
        int row = 0;
        foreach (var line in grid)
        {
            var col = 0;
            foreach (var controlInStack in line)
            {
                Add(controlInStack, col, row);
                col++;
            }
            row++;
        }
    }
}


public class DynamicGrid<T>
{
    private Dictionary<(int, int), T> grid = new Dictionary<(int, int), T>();
    private Dictionary<int, int> columnCountPerColumn = new Dictionary<int, int>();  // Stores row counts for each column
    private Dictionary<int, int> columnCountPerRow = new Dictionary<int, int>();  // Stores column counts for each row

    public int MaxRows { get; private set; } = 0;
    public int MaxColumns { get; private set; } = 0;

    public void Add(T item, int column, int row)
    {
        grid[(column, row)] = item;

        // Update the maximum rows and columns
        if (column >= MaxColumns)
            MaxColumns = column + 1;
        if (row >= MaxRows)
            MaxRows = row + 1;

        // Update the row count for the specified column
        if (columnCountPerColumn.TryGetValue(column, out int currentMaxRow))
        {
            columnCountPerColumn[column] = Math.Max(currentMaxRow, row + 1);
        }
        else
        {
            columnCountPerColumn[column] = row + 1;
        }

        // Update the column count for the specified row
        if (columnCountPerRow.TryGetValue(row, out int currentMaxColumn))
        {
            columnCountPerRow[row] = Math.Max(currentMaxColumn, column + 1);
        }
        else
        {
            columnCountPerRow[row] = column + 1;
        }
    }

    public T Get(int column, int row)
    {
        grid.TryGetValue((column, row), out T item);
        return item;
    }

    public IEnumerable<T> GetRow(int row)
    {
        for (int i = 0; i < MaxColumns; i++)
        {
            if (grid.TryGetValue((i, row), out T value))
            {
                yield return value;
            }
        }
    }

    public IEnumerable<T> GetColumn(int column)
    {
        for (int j = 0; j < MaxRows; j++)
        {
            if (grid.TryGetValue((column, j), out T value))
            {
                yield return value;
            }
        }
    }

    public IEnumerable<T> GetChildren()
    {
        return grid.Values;
    }

    public int GetCount()
    {
        return grid.Count;
    }

    /// <summary>
    /// Returns the column count for the specified row.
    /// This value is cached and updated each time an item is added.
    /// </summary>
    /// <param name="row">Row number to get the column count for.</param>
    /// <returns>Number of columns in the specified row.</returns>
    public int GetColumnCountForRow(int row)
    {
        if (columnCountPerRow.TryGetValue(row, out int count))
        {
            return count;
        }
        return 0; // Returns 0 if no columns are present for the row
    }
}

