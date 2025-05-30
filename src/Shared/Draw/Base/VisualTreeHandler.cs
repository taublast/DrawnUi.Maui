namespace DrawnUi.Draw
{
    public class VisualTreeHandler
    {
        #region DEBUG

        public void DumpPreparedTree()
        {
            if (PreparedTree!=null)
                DumpTree(PreparedTree);
        }

        public void DumpActiveTree()
        {
            if (ActiveTree!=null)
                DumpTree(ActiveTree);
        }

        public void DumpTree(VisualNode node, string prefix = "", bool isLast = true, int level = 0)
        {
            string indent = new string(' ', level * 4);

            // Box drawing characters for tree structure
            string connector = isLast ? "└─ " : "├─ ";
            string childPrefix = isLast ? "   " : "│  ";

            // Print the current node with appropriate prefix

            var controlInfo = node.Control.GetType().Name;
            if (node.Control is SkiaLayout layout)
            {
                controlInfo += $" {layout.Type}";
            }

            var line =
                $"{indent}{prefix}{connector}{controlInfo} at {node.HitBoxWithTransforms.Pixels.Location} ({node.Children.Count})";

            if (node.Cached != SkiaCacheType.None)
            {
                line += $" [{node.Cached}]";
            }

            if (!string.IsNullOrEmpty(node.Control.Tag))
            {
                line += $" - '{node.Control.Tag}'";
            }

            if (!string.IsNullOrEmpty(node.Tag))
            {
                line += $" by {node.Tag}";
            }

            Trace.WriteLine(line);

            // Process children
            for (int i = 0; i < node.Children.Count; i++)
            {
                bool childIsLast = (i == node.Children.Count - 1);
                DumpTree(node.Children[i], prefix + childPrefix, childIsLast, level);
            }
        }

        #endregion

        public bool IsReady
        {
            get
            {
                return PreparedTree != null;
            }
        }

        public bool WasRendered
        {
            get
            {
                return ActiveTree != null;
            }
        }

        /// <summary>
        /// This is used for rendering
        /// </summary>
        protected VisualNode ActiveTree;

        /// <summary>
        /// This is prepared and can be used to replace ActiveTree
        /// </summary>
        protected VisualNode PreparedTree;

        /// <summary>
        /// STEP 1 (or Background thread) prepare rendering tree that will be used for rendering later.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        public void PrepareRenderingTree(DrawingContext context, float widthRequest, float heightRequest, SkiaControl root)
        {
            // Build visual tree with transforms and caches
            var node = root.PrepareNode(context, widthRequest, heightRequest);

            // Push new tree
            PreparedTree = node;
        }

        /// <summary>
        /// STEP 2 (or Main thread) use prepared rendering tree to draw its nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        public void Render(DrawingContext context)
        {
            var prepared = PreparedTree;
            if (prepared != null)
            {
                ActiveTree = prepared;
            }

            RenderTreeInternal(context, ActiveTree);
        }

        /// <summary>
        /// Used by STEP 2 RenderTree method
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        protected void RenderTreeInternal(DrawingContext context, VisualNode node)
        {
            if (node != null)
            {
                node.Render(context);

                if (node.Cache == null)
                {
                    foreach (var child in node.Children)
                    {
                        RenderTreeInternal(context, child);
                    }
                }
            }
        }




       

    }
}
