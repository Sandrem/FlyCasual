using Ship;
using BoardTools;
using Arcs;
using Upgrade;
using System.Collections.Generic;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace FirstEdition.LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Asajj Ventress",
                    6,
                    37,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.AsajjVentressPilotAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
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