using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugManager
{
    public static bool ReleaseVersion = true;

    public static bool DebugNetworkSingleDevice;

    public static bool AlternativeCameraControls;

    public static bool NoCinematicCamera;

    public static bool FullDebug;

    public static bool DebugTemporary;

    public static bool DebugAllDamageIsCrits;

    public static bool DebugNoSquadPointsLimit;
    public static bool NoDefaultAiSquads;

    public static bool DebugNoCombat;
    public static bool DebugStraightToCombat;
    public static bool NoObstaclesSetup;

    public static bool DebugMovementShowTempBases; // = true;
    public static bool DebugMovementDestroyTempBasesLater; // = true;
    public static bool DebugMovementShowPlanning; // = true;

    public static bool FreeMode;
    public static bool BatchAiSquadTestingMode;
    public static bool BatchAiSquadTestingModeActive => BatchAiSquadTestingMode
        && Roster.GetPlayer(PlayerNo.Player1) is GenericAiPlayer
        && Roster.GetPlayer(PlayerNo.Player2) is GenericAiPlayer;

    public static bool ManualCollisionPrediction;

    public static bool DebugAiNavigation; // = true;

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
