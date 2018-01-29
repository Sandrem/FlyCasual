using Abilities;
using Ship;
using System;
using Upgrade;

namespace UpgradesList
{
    public class OutlawTech : GenericUpgrade
    {
        public OutlawTech() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Outlaw Tech";
            Cost = 2;

            UpgradeAbilities.Add(new OutlawTechAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
{
    public class OutlawTechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterOutlawTechAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterOutlawTechAbility;
        }

        private void RegisterOutlawTechAbility(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == Movement.ManeuverColor.Red)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskAssignFocusToken);
            }
        }

        private void AskAssignFocusToken(object sender, EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                HostShip.AssignToken(new Tokens.FocusToken(HostShip), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, EventArgs e)
        {
            HostShip.AssignToken(new Tokens.FocusToken(HostShip), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}