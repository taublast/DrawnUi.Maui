#if MACCATALYST


namespace DrawnUi.Draw;

public partial class KeyboardManager
{

    /*
      public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        foreach (UIPress press in presses)
        {
            var mapped = AppleKeyMapper.MapToMaui((int)press.Type);
            KeyboardManager.KeyboardPressed(mapped);
        }

        //if (!consumed)
        //{
        //    base.PressesBegan(presses, evt);
        //}
    }

    void ReleaseKeys(NSSet<UIPress> presses)
    {
        foreach (UIPress press in presses)
        {
            var mapped = AppleKeyMapper.MapToMaui((int)press.Type);
            KeyboardManager.KeyboardReleased(mapped);

            //Trace.WriteLine($"[KEY] {press.Type}/{(int)press.Type} => {mapped}");
        }
    }

    public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        base.PressesEnded(presses, evt);

        ReleaseKeys(presses);
    }
     */

    public static MauiKey MapToMaui(int virtualKey)
    {
        switch (virtualKey)
        {
        case 2044: return MauiKey.Space;
        case 2079: return MauiKey.ArrowRight;
        case 2080: return MauiKey.ArrowLeft;
        case 2081: return MauiKey.ArrowDown;
        case 2082: return MauiKey.ArrowUp;
        case 2030: return MauiKey.Digit0;
        case 2031: return MauiKey.Digit1;
        case 2032: return MauiKey.Digit2;
        case 2033: return MauiKey.Digit3;
        case 2034: return MauiKey.Digit4;
        case 2035: return MauiKey.Digit5;
        case 2036: return MauiKey.Digit6;
        case 2037: return MauiKey.Digit7;
        case 2038: return MauiKey.Digit8;
        case 2039: return MauiKey.Digit9;

        case 2004: return MauiKey.KeyA;
        case 2005: return MauiKey.KeyB;
        case 2006: return MauiKey.KeyC;
        case 2007: return MauiKey.KeyD;
        case 2008: return MauiKey.KeyE;
        case 2009: return MauiKey.KeyF;
        case 2010: return MauiKey.KeyG;
        case 2011: return MauiKey.KeyH;
        case 2012: return MauiKey.KeyI;
        case 2013: return MauiKey.KeyJ;
        case 2014: return MauiKey.KeyK;
        case 2015: return MauiKey.KeyL;
        case 2016: return MauiKey.KeyM;
        case 2017: return MauiKey.KeyN;
        case 2018: return MauiKey.KeyO;
        case 2019: return MauiKey.KeyP;
        case 2020: return MauiKey.KeyQ;
        case 2021: return MauiKey.KeyR;
        case 2022: return MauiKey.KeyS;
        case 2023: return MauiKey.KeyT;
        case 2024: return MauiKey.KeyU;
        case 2025: return MauiKey.KeyV;
        case 2026: return MauiKey.KeyW;
        case 2027: return MauiKey.KeyX;
        case 2028: return MauiKey.KeyY;
        case 2029: return MauiKey.KeyZ;

        case 2057: return MauiKey.CapsLock;

        case 2117: return MauiKey.Insert;
        //case VirtualKey.Delete: return MauiKey.Delete;
        case 2104: return MauiKey.PrintScreen;
        case 2074: return MauiKey.Home;
        case 2077: return MauiKey.End;

        case 2075: return MauiKey.PageUp;
        case 2078: return MauiKey.PageDown;

        case 2041: return MauiKey.Escape;
        //case VirtualKey.Pause: return MauiKey.Pause;
        case 2226: return MauiKey.AltLeft;
        case 2230: return MauiKey.AltRight;
        case 2225: return MauiKey.ShiftLeft;
        case 2229: return MauiKey.ShiftRight;

        case 2100: return MauiKey.IntBackslash;
        case 2053: return MauiKey.Backquote;

        case 2228: return MauiKey.ControlRight;
        case 2224: return MauiKey.ControlLeft;
        case 2040: return MauiKey.Enter;
        case 2043: return MauiKey.Tab;
        case 2042: return MauiKey.Backspace;
        case 2046: return MauiKey.Equal;
        case 2045: return MauiKey.Minus;
        case 2058: return MauiKey.F1;
        case 2059: return MauiKey.F2;
        case 2060: return MauiKey.F3;
        case 2061: return MauiKey.F4;
        case 2062: return MauiKey.F5;
        case 2063: return MauiKey.F6;
        case 2064: return MauiKey.F7;
        case 2065: return MauiKey.F8;
        case 2066: return MauiKey.F9;
        case 2067: return MauiKey.F10;
        case 2068: return MauiKey.F11;
        case 2069: return MauiKey.F12;
        case 2083: return MauiKey.NumLock;
        //case VirtualKey.Scroll: return MauiKey.ScrollLock;
        case 2098: return MauiKey.Numpad0;
        case 2089: return MauiKey.Numpad1;
        case 2090: return MauiKey.Numpad2;
        case 2091: return MauiKey.Numpad3;
        case 2092: return MauiKey.Numpad4;
        case 2093: return MauiKey.Numpad5;
        case 2094: return MauiKey.Numpad6;
        case 2095: return MauiKey.Numpad7;
        case 2096: return MauiKey.Numpad8;
        case 2097: return MauiKey.Numpad9;

        //case CommandLeft: return MauiKey.MetaLeft;

        case 2084: return MauiKey.NumpadDivide;
        case 2085: return MauiKey.NumpadMultiply;
        case 2086: return MauiKey.NumpadSubtract;
        case 2087: return MauiKey.NumpadAdd;

        case 2054: return MauiKey.Comma;
        case 2055: return MauiKey.Period;
        case 2056: return MauiKey.Slash;

        case 2047: return MauiKey.BracketLeft;
        case 2048: return MauiKey.BracketRight;
        case 2049: return MauiKey.Backslash;
        case 2052: return MauiKey.Quote;
        case 2051: return MauiKey.Semicolon;


        }

        return MauiKey.Unknown;

    }

}

#endif