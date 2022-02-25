using Abilities.SecondEdition;
using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class MagvaYarro : UT60DUWing
        {
            public MagvaYarro() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Magva Yarro",
                    "Cavern Angels Spotter",
                    Faction.Rebel,
                    3,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(MagvaYarroPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Partisan
                    },
                    seImageNumber: 57,
                    skinName: "Partisan"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagvaYarroPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            DiceRerollManager.OnMaxDiceRerollAllowed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            DiceRerollManager.OnMaxDiceRerollAllowed -= CheckAbility;
        }

        private void CheckAbility(ref int maxDiceRerollCount)
        {
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distInfo.Range > 2) return;

            if (maxDiceRerollCount > 1) maxDiceRerollCount = 1;
        }
    }
}
