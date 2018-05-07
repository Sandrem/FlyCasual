using GameModes;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RuleSets
{
    interface ISecondEditionShip
    {
        void AdaptShipToSecondEdition();
    }

    interface ISecondEditionPilot
    {
        void AdaptPilotToSecondEdition();
    }

    public class SecondEdition : RuleSet
    {
        public override string Name { get { return "Second Edition"; } }

        public override int MaxPoints { get { return 200; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }

        public override void EvadeDiceModification(DiceRoll diceRoll)
        {
            if (diceRoll.Blanks > 0)
            {
                diceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            }
            else if (diceRoll.Focuses > 0)
            {
                diceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            }
            else
            {
                Messages.ShowError("Evade Token is spent, but no effect");
            }
        }

        public override void ActionIsFailed(GenericShip ship, Type actionType)
        {
            Messages.ShowError("Action is failed and skipped");
            Phases.CurrentSubPhase.PreviousSubPhase.Resume();
            GameMode.CurrentGameMode.SkipButtonEffect();
        }

        public override bool ShipIsAllowed(GenericShip ship)
        {
            return ship.ShipRuleType == typeof(SecondEdition);
        }

        public override bool PilotIsAllowed(GenericShip ship)
        {
            return ship.PilotRuleType == typeof(SecondEdition);
        }

        public override void AdaptShipToRules(GenericShip ship)
        {
            if (ship is ISecondEditionShip)
            {
                (ship as ISecondEditionShip).AdaptShipToSecondEdition();
                ship.ShipRuleType = typeof(SecondEdition);
            }
        }

        public override void AdaptPilotToRules(GenericShip ship)
        {
            if (ship is ISecondEditionPilot)
            {
                (ship as ISecondEditionPilot).AdaptPilotToSecondEdition();
                ship.PilotRuleType = typeof(SecondEdition);
            }
        }
    }
}
