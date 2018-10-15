using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using SubPhases;
using BoardTools;
using Abilities;
using System.Linq;
using Arcs;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft, ISecondEditionPilot
        {
            public AsajjVentress() : base()
            {
                PilotName = "Asajj Ventress";
                PilotSkill = 6;
                Cost = 37;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new AsajjVentressPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 84;
                MaxForce = 2;

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);

                PilotAbilities.RemoveAll(ability => ability is Abilities.AsajjVentressPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.AsajjVentressPilotAbilitySE());

                SEImageNumber = 219;
            }
        }
    }
}

namespace Abilities
{
    public class AsajjVentressPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterAsajjVentressPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterAsajjVentressPilotAbility;
        }

        protected virtual void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected virtual void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a ship inside your mobile firing arc to assign Stress token to it.",
                HostShip
            );
        }

        protected bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 2) && FilterTargetInMobileFiringArc(ship);
        }

        protected int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            priority += (ship.Tokens.CountTokensByType(typeof(StressToken)) * 25);
            priority += (ship.Agility * 5);

            if (ship.CanPerformActionsWhileStressed && ship.CanPerformRedManeuversWhileStressed) priority = 10;

            return priority;
        }

        private bool FilterTargetInMobileFiringArc(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
            return shotInfo.InArcByType(ArcTypes.Mobile);
        }

        private void CheckAssignStress()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, TargetShip, HostShip.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcTypes.Mobile) && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Stress Token\nto " + TargetShip.PilotName);
                TargetShip.Tokens.AssignToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InArcByType(ArcTypes.Mobile)) Messages.ShowError("Target is not inside Mobile Arc");
                else if (shotInfo.Range >= 3) Messages.ShowError("Target is outside range 2");
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class AsajjVentressPilotAbilitySE : AsajjVentressPilotAbility
    {

        protected override void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility) && HostShip.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected override void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a ship inside your mobile firing arc to assign Stress token to it.",
                HostShip
            );
        }

        private void CheckAssignStress()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            AsajjVentressAbilityDecisionSubPhaseSE subphase = (AsajjVentressAbilityDecisionSubPhaseSE)
                Phases.StartTemporarySubPhaseNew(
                "Choose effect of Asajj Ventress' ability.",
                typeof(AsajjVentressAbilityDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );

            Selection.ThisShip = TargetShip;
            Selection.ActiveShip = HostShip;
            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class AsajjVentressAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            InfoText = Selection.ThisShip.ShipId + ": " + "Select the effect of Asajj Ventress' ability.";
            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = "Recieve a stress token.";

            AddDecision("Recieve a stress token.", RecieveStress);
        }

        private void RecieveStress(object sender, System.EventArgs e)
        {
            Selection.ActiveShip.Force--;
            Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}