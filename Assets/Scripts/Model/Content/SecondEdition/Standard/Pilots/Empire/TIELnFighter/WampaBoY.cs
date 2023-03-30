using System.Collections.Generic;
using Ship;
using Abilities.SecondEdition;
using Upgrade;
using Content;
using Actions;
using ActionsList;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class WampaBoY : TIELnFighter
        {
            public WampaBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Wampa\"",
                    "Battle of Yavin",
                    Faction.Imperial,
                    1,
                    3,
                    0,
                    isLimited: true,
                    abilityType: typeof(WampaAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(TargetLockAction)));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/0c/Wampa-battleofyavin.png";

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Elusive));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Vengeful));

                ShipInfo.Hull++;

                PilotNameCanonical = "wampa-battleofyavin";
            }
        }
    }
}