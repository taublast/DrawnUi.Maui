namespace DrawnUi.Draw;

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