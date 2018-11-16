using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Asajj Ventress",
                    4,
                    84,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.AsajjVentressPilotAbility),
                    force: 2
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Force);

                SEImageNumber = 219;
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            priority += (ship.State.Agility * 5);

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
    public class AsajjVentressPilotAbility : Abilities.FirstEdition.AsajjVentressPilotAbility
    {

        protected override void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility) && HostShip.State.Force > 0)
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
            Selection.ActiveShip.State.Force--;
            Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}