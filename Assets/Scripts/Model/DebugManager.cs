using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugManager
{
    public static bool FullDebug;

    private static bool debugPhases;
    public static bool DebugPhases
    {
        get {
            if (FullDebug) return true;
            return debugPhases;
        }
        set { debugPhases = value; }
    }

    private static bool debugTriggers;
    public static bool DebugTriggers
    {
        get
        {
            if (FullDebug) return true;
            return debugTriggers;
        }
        set { debugTriggers = value; }
    }

    private static bool debugAI;
    public static bool DebugAI
    {
        get
        {
            if (FullDebug) return true;
            return debugAI;
        }
        set { debugAI = value; }
    }

    private static bool debugBoard;
    public static bool DebugBoard
    {
        get
        {
            if (FullDebug) return true;
            return debugBoard;
        }
        set { debugBoard = value; }
    }

}
