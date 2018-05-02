using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RulesList;

public static class Rules
{
    public static WinConditionsRule WinConditions { get; private set; }
    public static DistanceBonusRule DistanceBonus { get; private set; }
    public static EndPhaseCleanupRule EndPhase { get; private set; }
    public static StressRule Stress { get; private set; }
    public static OffTheBoardRule OffTheBoard { get; private set; }
    public static CollisionRules Collision { get; private set; }
    public static DuplicatedActionsRule DuplicatedActions { get; private set; }
    public static AsteroidLandedRule AsteroidLanded { get; private set; }
    public static AsteroidHitRule AsteroidHit { get; private set; }
    public static MineHitRule MineHit { get; private set; }
    public static AsteroidObstructionRule AsteroidObstruction { get; private set; }
    public static InitiativeRule Initiative { get; private set; }
    public static TargetIsLegalForShotRule TargetIsLegalForShot { get; private set; }
    public static IonizationRule Ionization { get; private set; }
    public static JammedRule Jammed { get; private set; }
    public static TargetLocksRule TargetLocks { get; private set; }
    public static WeaponsDisabledRule WeaponsDisabled { get; private set; }
    public static BullseyeArcRule BullseyeArc { get; private set; }
    public static DockingRule Docking { get; private set; }
    public static TractorBeamRule TractorBeam { get; private set; }

    static Rules()
    {
        WinConditions = new WinConditionsRule();
        DistanceBonus = new DistanceBonusRule();
        EndPhase = new EndPhaseCleanupRule();
        Stress = new StressRule();
        OffTheBoard = new OffTheBoardRule();
        Collision = new CollisionRules();
        DuplicatedActions = new DuplicatedActionsRule();
        AsteroidLanded = new AsteroidLandedRule();
        AsteroidHit = new AsteroidHitRule();
        MineHit = new MineHitRule();
        AsteroidObstruction = new AsteroidObstructionRule();
        Initiative = new InitiativeRule();
        TargetIsLegalForShot = new TargetIsLegalForShotRule();
        Ionization = new IonizationRule();
        Jammed = new JammedRule();
        TargetLocks = new TargetLocksRule();
        WeaponsDisabled = new WeaponsDisabledRule();
        BullseyeArc = new BullseyeArcRule();
        Docking = new DockingRule();
        TractorBeam = new TractorBeamRule();
    }

    public static void FinishGame()
    {
        Docking.Initialize();
        Phases.EndGame();
    }
}

