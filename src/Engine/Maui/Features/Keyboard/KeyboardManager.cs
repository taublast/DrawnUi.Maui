using System.Diagnostics;

namespace DrawnUi.Maui.Draw;


public partial class KeyboardManager
{

    public static event EventHandler<MauiKey> KeyDown;

    public static event EventHandler<MauiKey> KeyUp;

    public static void KeyboardPressed(MauiKey key)
    {
        CheckAndApplyModifiers(key, true);

        //Debug.WriteLine($"[KEY UP] {key}");

        KeyDown?.Invoke(null, key);
    }

    public static void KeyboardReleased(MauiKey key)
    {
        CheckAndApplyModifiers(key, false);

        KeyUp?.Invoke(null, key);
    }

    public static bool IsShiftPressed
    {
        get
        {
            return IsLeftShiftDown || IsRightShiftDown;
        }
    }

    public static bool IsAltPressed
    {
        get
        {
            return IsLeftAltDown || IsRightAltDown;
        }
    }

    public static bool IsControlPressed
    {
        get
        {
            return IsLeftControlDown || IsRightControlDown;
        }
    }

    static bool IsLeftShiftDown { get; set; }

    static bool IsRightShiftDown { get; set; }

    static bool IsLeftAltDown { get; set; }

    static bool IsRightAltDown { get; set; }

    static bool IsLeftControlDown { get; set; }

    static bool IsRightControlDown { get; set; }

    static void CheckAndApplyModifiers(MauiKey key, bool state)
    {
        if (key == MauiKey.ShiftLeft)
        {
            IsLeftShiftDown = state;
        }
        else
        if (key == MauiKey.ShiftRight)
        {
            IsRightShiftDown = state;
        }
        else
        if (key == MauiKey.AltLeft)
        {
            IsLeftAltDown = state;
        }
        else
        if (key == MauiKey.AltRight)
        {
            IsRightAltDown = state;
        }
        else
        if (key == MauiKey.ControlLeft)
        {
            IsLeftControlDown = state;
        }
        else
        if (key == MauiKey.ControlRight)
        {
            IsRightControlDown = state;
        }
    }


}