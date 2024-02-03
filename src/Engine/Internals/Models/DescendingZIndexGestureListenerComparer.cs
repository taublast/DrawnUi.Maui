using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Draw;

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