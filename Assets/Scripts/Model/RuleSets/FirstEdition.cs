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

        public override Dictionary<Type, int> DamageDeckContent {
            get
            {
                return new Dictionary<Type, int>()
                {
                    { typeof(DamageDeckCardFE.DirectHit),           7 },
                    { typeof(DamageDeckCardFE.BlindedPilot),        2 },
                    { typeof(DamageDeckCardFE.DamagedCockpit),      2 },
                    { typeof(DamageDeckCardFE.DamagedEngine),       2 },
                    { typeof(DamageDeckCardFE.DamagedSensorArray),  2 },
                    { typeof(DamageDeckCardFE.LooseStabilizer),     2 },
                    { typeof(DamageDeckCardFE.MajorHullBreach),     2 },
                    { typeof(DamageDeckCardFE.ShakenPilot),         2 },
                    { typeof(DamageDeckCardFE.StructuralDamage),    2 },
                    { typeof(DamageDeckCardFE.ThrustControlFire),   2 },
                    { typeof(DamageDeckCardFE.WeaponsFailure),      2 },
                    { typeof(DamageDeckCardFE.ConsoleFire),         2 },
                    { typeof(DamageDeckCardFE.StunnedPilot),        2 },
                    { typeof(DamageDeckCardFE.MajorExplosion),      2 }
                };
            }
        }

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
