using Abilities;
using Ship;
using System;
using Tokens;
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

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(50, 4));

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
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (HostShip.GetLastManeuverColor() == Movement.MovementComplexity.Complex)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskAssignFocusToken);
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
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}