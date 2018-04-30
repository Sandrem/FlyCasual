using Abilities;
using Ship;
using SubPhases;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class KyleKatarn : GenericUpgrade
    {
        public KyleKatarn() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Kyle Katarn";
            Cost = 3;

            isUnique = true;

            // AvatarOffset = new Vector2(42, 1);

            UpgradeAbilities.Add(new KyleKatarnCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class KyleKatarnCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsRemoved += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsRemoved -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsRemoved, AskAssignFocusToken);
            }
        }

        private void AskAssignFocusToken(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AssignFocusToken, null, null, true);
        }

        private void AssignFocusToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new Tokens.FocusToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }
    }
}