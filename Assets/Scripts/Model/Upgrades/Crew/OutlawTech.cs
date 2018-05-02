using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class OutlawTech : GenericUpgrade
    {
        public OutlawTech() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Outlaw Tech";
            Cost = 2;

            AvatarOffset = new Vector2(50, 4);

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
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

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
                HostShip.Tokens.AssignToken(new Tokens.FocusToken(HostShip), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new Tokens.FocusToken(HostShip), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}