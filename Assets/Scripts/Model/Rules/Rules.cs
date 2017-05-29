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
    public static KoiogranTurnRule KoiogranTurn { get; private set; }
    public static CollisionRules Collision { get; private set; }
    public static FiringRangeLimit FiringRange { get; private set; }
    public static FiringArcRule FiringArc { get; private set; }
    public static DuplicatedActionsRule DuplicatedActions { get; private set; }
    public static AsteroidLandedRule AsteroidLanded { get; private set; }

    private static GameManagerScript Game;

    static Rules()
    {
        Game = UnityEngine.GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        WinConditions = new WinConditionsRule(Game);
        DistanceBonus = new DistanceBonusRule(Game);
        EndPhase = new EndPhaseCleanupRule(Game);
        Stress = new StressRule();
        OffTheBoard = new OffTheBoardRule(Game);
        KoiogranTurn = new KoiogranTurnRule(Game);
        Collision = new CollisionRules(Game);
        FiringRange = new FiringRangeLimit(Game);
        FiringArc = new FiringArcRule(Game);
        DuplicatedActions = new DuplicatedActionsRule();
        AsteroidLanded = new AsteroidLandedRule(Game);
    }
}

