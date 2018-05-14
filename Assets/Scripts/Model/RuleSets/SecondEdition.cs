using ActionsList;
using Arcs;
using GameModes;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Upgrade;

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

    interface ISecondEditionUpgrade
    {
        void AdaptUpgradeToSecondEdition();
    }

    public class SecondEdition : RuleSet
    {
        public override string Name { get { return "Second Edition"; } }

        public override int MaxPoints { get { return 200; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Engagement"; } }
        public override Color MovementEasyColor { get { return new Color(0, 1, 1); } }

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

        public override void AdaptUpgradeToRules(GenericUpgrade upgrade)
        {
            if (upgrade is ISecondEditionUpgrade)
            {
                (upgrade as ISecondEditionUpgrade).AdaptUpgradeToSecondEdition();
                upgrade.Charges = upgrade.MaxCharges;

                upgrade.UpgradeRuleType = typeof(SecondEdition);
            }
        }

        public override bool WeaponHasRangeBonus()
        {
            return Combat.ChosenWeapon is PrimaryWeaponClass || (Combat.ChosenWeapon as GenericUpgrade).Types.Contains(UpgradeType.Cannon) || (Combat.ChosenWeapon as GenericUpgrade).Types.Contains(UpgradeType.Turret);
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageSecondEdition();
        }

        public override void RotateMobileFiringArc(ArcFacing facing)
        {
            Selection.ThisShip.ShowMobileFiringArcHighlight(facing);
        }

        public override void ActivateGenericUpgradeAbility(GenericShip host, List<UpgradeType> upgradeTypes)
        {
            if (upgradeTypes.Contains(UpgradeType.Turret))
            {
                host.ShipBaseArcsType = BaseArcsType.ArcMobile;
                host.InitializeShipBaseArc();

                // Temporary
                if (!host.PrintedActions.Any(n => n.GetType() == typeof(RotateArcAction))) host.PrintedActions.Add(new RotateArcAction());
            }
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
        }
    }
}
