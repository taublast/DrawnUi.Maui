using System.Runtime.InteropServices;

namespace DrawnUi.Draw;

public class DynamicGrid<T>
{
    protected Dictionary<(int, int), T> grid = new Dictionary<(int, int), T>();
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

    public void Clear()
    {
        grid.Clear();
        columnCountPerColumn.Clear();
        columnCountPerRow.Clear();
    }

    public IEnumerable<T> GetChildren()
    {
        return grid.Values;
    }

    public T FindChildAtIndex(int index)
    {
        return grid.Values.ToArray()[index];
    }

    public Span<T> GetChildrenAsSpans()
    {
        return CollectionsMarshal.AsSpan(grid.Values.ToList());
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
