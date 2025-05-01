namespace DrawnUi.Draw
{
    public class RenderedNode
    {

        public RenderedNode(IDrawnBase control)
        {
            Control = control;
            Children = new();
        }

        public IDrawnBase Control { get;set; }

        public List<RenderedNode> Children { get; set; }
    }
}
