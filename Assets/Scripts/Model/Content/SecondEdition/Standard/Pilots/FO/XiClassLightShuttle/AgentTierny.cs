using System.Collections.Generic;
using Upgrade;
using Abilities.Parameters;

namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class AgentTierny : XiClassLightShuttle
        {
            public AgentTierny() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Agent Tierny",
                    "Persuasive Recruiter",
                    Faction.FirstOrder,
                    3,
                    5,
                    15,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    abilityType: typeof(Abilities.SecondEdition.AgentTiernyPilotAbility)
                );

                ImageUrl = "https://i.imgur.com/7zFrncC.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgentTiernyPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}

