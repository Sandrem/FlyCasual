using Ship;
using Upgrade;
using UnityEngine;
using System;
using Tokens;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class KyleKatarn : GenericUpgrade
    {
        public KyleKatarn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kyle Katarn",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.KyleKatarnCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(42, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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
            HostShip.Tokens.AssignToken(typeof(FocusToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}