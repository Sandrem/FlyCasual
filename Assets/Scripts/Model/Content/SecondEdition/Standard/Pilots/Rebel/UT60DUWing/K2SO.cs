using Abilities.SecondEdition;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class K2SO : UT60DUWing
        {
            public K2SO() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "K-2SO",
                    "Cassian Said I Had To",
                    Faction.Rebel,
                    3,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(K2SOPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/56/cb/56cb9ec8-0eff-4c6f-acda-f54413baadc7/swz66_k-2so.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class K2SOPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterK2SOAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterK2SOAbility;
        }

        private void RegisterK2SOAbility(GenericShip ship, GenericToken token)
        {
            if (token is StressToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AssignToken);
            }
        }

        private void AssignToken(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Calculate Token is assigned");
            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }
    }
}
