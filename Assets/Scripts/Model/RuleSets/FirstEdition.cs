using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Upgrade;

namespace RuleSets
{
    public class FirstEdition : RuleSet
    {
        public override string Name { get { return "First Edition"; } }

        public override int MaxPoints { get { return 100; } }
        public override int MinShipCost { get { return 14; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Combat"; } }
        public override Color MovementEasyColor { get { return Color.green; } }

        public override void EvadeDiceModification(DiceRoll diceRoll)
        {
            diceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
        }

        public override void ActionIsFailed(GenericShip ship, Type actionType)
        {
            ship.RemoveAlreadyExecutedAction(actionType);
            Phases.CurrentSubPhase.PreviousSubPhase.Resume();
        }

        public override bool PilotIsAllowed(GenericShip ship)
        {
            return ship.PilotRuleType == typeof(FirstEdition);
        }

        public override bool ShipIsAllowed(GenericShip ship)
        {
            return ship.ShipRuleType == typeof(FirstEdition);
        }

        public override bool WeaponHasRangeBonus()
        {
            return Combat.ChosenWeapon is PrimaryWeaponClass;
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageFirstEdition();
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).PerfromTemplatePlanningFirstEdition();
        }

        public override void SubScribeToGenericShipEvents(GenericShip ship)
        {
            ship.OnTryAddAvailableActionEffect += Rules.BullseyeArc.CheckBullseyeArc;
        }
    }
}
