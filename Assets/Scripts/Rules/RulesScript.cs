using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rules
{
    public class RulesScript
    {
        public WinConditionsRule WinConditions { get; private set; }
        public DistanceBonusRule DistanceBonus { get; private set; }
        public EndPhaseRule EndPhase { get; private set; }
        public StressRule Stress { get; private set; }
        public OffTheBoardRule OffTheBoard { get; private set; }
        public KoiogranTurnRule KoiogranTurn { get; private set; }
        public CollisionRules Collision { get; private set; }
        public FiringRangeLimit FiringRange { get; private set; }
        public FiringArcRule FiringArc { get; private set; }

        private GameManagerScript Game;

        public RulesScript(GameManagerScript game)
        {
            Game = game;

            WinConditions = new WinConditionsRule(Game);
            DistanceBonus = new DistanceBonusRule(Game);
            EndPhase = new EndPhaseRule(Game);
            Stress = new StressRule(Game);
            OffTheBoard = new OffTheBoardRule(Game);
            KoiogranTurn = new KoiogranTurnRule(Game);
            Collision = new CollisionRules(Game);
            FiringRange = new FiringRangeLimit(Game);
            FiringArc = new FiringArcRule(Game);
        }
    }
}

