
namespace DrawnUi.Maui.Draw
{
	public partial class SkiaLayout
	{
		/*
		#region HOTRELOAD

		public override void ReportHotreloadChildRemoved(SkiaControl control)
		{
			if (control == null)
				return;

			try
			{
				var index = this.ChildrenFactory.GetChildrenCount();
				if (IsStack)
				{
					if (RenderTree == null)
						index = -1;
					else
						index = RenderTree.Count; //not Length-1 cuz already removed from RenderTree
					VisualDiagnostics.OnChildRemoved(this, control, index);
				}
				else
				{
					base.ReportHotreloadChildRemoved(control);
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine("-------------------------------------------------");
				Trace.WriteLine($"[HOTRELOAD] Exception OnChildRemoved {Tag} {e}");
				Trace.WriteLine("-------------------------------------------------");
			}
		}

		public override void ReportHotreloadChildAdded(SkiaControl child)
		{
			if (child == null)
				return;

			try
			{
				var index = GetVisualChildren().FindIndex(child);
				VisualDiagnostics.OnChildAdded(this, child, index);
			}
			catch (Exception e)
			{
				Trace.WriteLine("-------------------------------------------------");
				Trace.WriteLine($"[HOTRELOAD] Exception ReportHotreloadChildAdded {Tag} {e}");
				Trace.WriteLine("-------------------------------------------------");
			}
		}

		/// For Xaml HotReload. This is semetimes not called when we add and remove views.
		/// </summary>
		/// <returns></returns>
		public override IReadOnlyList<IVisualTreeElement> GetVisualChildren()
		{

			try
			{
				return base.GetVisualChildren();
			}
			catch (Exception e)
			{
				Super.Log("-------------------------------------------------");
				Super.Log($"[HOTRELOAD] Exception GetVisualChildren {Tag} {e}");
				Super.Log("-------------------------------------------------");
				return base.GetVisualChildren();
			}

		}

		#endregion
		*/

	}
}
