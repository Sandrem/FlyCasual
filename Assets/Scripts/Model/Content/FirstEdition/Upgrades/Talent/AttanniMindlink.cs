using Ship;
using Upgrade;
using UnityEngine;
using Tokens;
using System;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class AttanniMindlink : GenericUpgrade
    {
        public AttanniMindlink() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Attanni Mindlink",
                UpgradeType.Talent,
                cost: 1,
                limited: 2,
                abilityType: typeof(Abilities.FirstEdition.AttanniMindlinkAbility),
                restriction: new FactionRestriction(Faction.Scum)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector3(70, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AttanniMindlinkAbility : GenericAbility
    {
        private class TokensArgs : EventArgs
        {
            public Type TokenType;
        }

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterAttanniMindlinkAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterAttanniMindlinkAbility;
        }

        private void RegisterAttanniMindlinkAbility(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(FocusToken) || tokenType == typeof(StressToken))
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnTokenIsAssigned,
                    CheckAttanniMindlinkAbility,
                    new TokensArgs { TokenType = tokenType }
                );
            }
        }

        private void CheckAttanniMindlinkAbility(object sender, System.EventArgs e)
        {
            bool tokenMustBeAssigned = false;

            Type tokenType = (e as TokensArgs).TokenType;
            foreach (var friendlyShip in HostShip.Owner.Ships)
            {
                if (HasAttanniMinklink(friendlyShip.Value))
                {
                    if (friendlyShip.Value.ShipId != HostShip.ShipId)
                    {
                        if (!friendlyShip.Value.Tokens.HasToken(tokenType))
                        {
                            if (tokenType == typeof(FocusToken))
                            {
                                tokenMustBeAssigned = true;
                                friendlyShip.Value.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
                                break;
                            }
                            else if (tokenType == typeof(StressToken))
                            {
                                tokenMustBeAssigned = true;
                                friendlyShip.Value.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
                                break;
                            }
                        }
                    }
                }
            }

            if (!tokenMustBeAssigned) Triggers.FinishTrigger();
        }

        private bool HasAttanniMinklink(GenericShip ship)
        {
            return ship.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n.GetType() == HostUpgrade.GetType()) == 1;
        }

    }
}