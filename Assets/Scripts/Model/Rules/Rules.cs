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
    public static FiringRangeLimit FiringRange { get; private set; }
    public static FiringArcRule FiringArc { get; private set; }
    public static DuplicatedActionsRule DuplicatedActions { get; private set; }
    public static AsteroidLandedRule AsteroidLanded { get; private set; }
    public static AsteroidHitRule AsteroidHit { get; private set; }
    public static AsteroidObstructionRule AsteroidObstruction { get; private set; }
    public static InitiativeRule Initiative { get; private set; }
    public static TargetIsLegalForShotRule TargetIsLegalForShot { get; private set; }

    private static GameManagerScript Game;

    static Rules()
    {
        Game = UnityEngine.GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        WinConditions = new WinConditionsRule(Game);
        DistanceBonus = new DistanceBonusRule();
        EndPhase = new EndPhaseCleanupRule();
        Stress = new StressRule();
        OffTheBoard = new OffTheBoardRule(Game);
        Collision = new CollisionRules();
        FiringRange = new FiringRangeLimit();
        FiringArc = new FiringArcRule();
        DuplicatedActions = new DuplicatedActionsRule();
        AsteroidLanded = new AsteroidLandedRule();
        AsteroidHit = new AsteroidHitRule();
        AsteroidObstruction = new AsteroidObstructionRule();
        Initiative = new InitiativeRule();
        TargetIsLegalForShot = new TargetIsLegalForShotRule();
    }

}

