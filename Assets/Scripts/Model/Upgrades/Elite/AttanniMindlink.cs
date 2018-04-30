using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;
using Tokens;
using System.Linq;
using SquadBuilderNS;

namespace UpgradesList
{
    public class AttanniMindlink : GenericUpgrade
    {
        public AttanniMindlink() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Attanni Mindlink";
            Cost = 1;

            AvatarOffset = new Vector3(70, 1);

            UpgradeAbilities.Add(new AttanniMindlinkAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            int sameUpgradesInstalled = 0;
            foreach (var ship in squadList.GetShips())
            {
                foreach (var upgrade in ship.Instance.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    if (upgrade.GetType() == this.GetType()) sameUpgradesInstalled++;
                }
            };
            if (sameUpgradesInstalled > 2) Messages.ShowError("Cannot have more than 2 Attanni Mindlinks");
            return sameUpgradesInstalled < 3;
        }
    }
}

namespace Abilities
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
                                friendlyShip.Value.Tokens.AssignToken(new FocusToken(friendlyShip.Value), Triggers.FinishTrigger);
                                break;
                            }
                            else if (tokenType == typeof(StressToken))
                            {
                                tokenMustBeAssigned = true;
                                friendlyShip.Value.Tokens.AssignToken(new StressToken(friendlyShip.Value), Triggers.FinishTrigger);
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