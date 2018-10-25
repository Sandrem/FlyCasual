using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugManager
{
    public static bool ReleaseVersion = false;

    public static bool FullDebug;

    public static bool DebugTemporary;

    public static bool DebugAllDamageIsCrits;

    public static bool DebugNoSquadPointsLimit;

    public static bool DebugNoCombat;

    public static bool NoAsteroidSetup;

    public static bool NoReplayCreation;

    public static bool DebugMovement;

    public static bool ErrorIsAlreadyReported { get; set; }

    private static bool debugNetwork = true;
    public static bool DebugNetwork
    {
        get
        {
            if (FullDebug) return true;
            return debugNetwork;
        }
        set { debugNetwork = value; }
    }

    private static bool debugPhases;
    public static bool DebugPhases
    {
        get {
            if (FullDebug) return true;
            return debugPhases;
        }
        set { debugPhases = value; }
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

    private static bool debugDamage;
    public static bool DebugDamage
    {
        get
        {
            if (FullDebug) return true;
            return debugDamage;
        }
        set { debugDamage = value; }
    }

}
