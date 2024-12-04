namespace DrawnUi.Maui.Draw;

public class LayoutStructure : DynamicGrid<ControlInStack>
{
    public LayoutStructure()
    {

    }

    public ControlInStack GetForIndex(int index)
    {
        return grid.Values.FirstOrDefault(x => x.ControlIndex == index);
    }

    public LayoutStructure(List<List<ControlInStack>> grid)
    {
        int row = 0;
        foreach (var line in grid)
        {
            var col = 0;
            foreach (var controlInStack in line)
            {
                controlInStack.Column = col;
                controlInStack.Row = row;

                Add(controlInStack, col, row);
                col++;
            }
            row++;
        }
    }
}
