using Abilities.SecondEdition;
using BoardTools;
using Conditions;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntillesBoY : T65XWing
        {
            public WedgeAntillesBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wedge Antilles",
                    "Battle of Yavin",
                    Faction.Rebel,
                    5,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(WedgeAntillesBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Wedge Antilles",
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(AttackSpeed));
                MustHaveUpgrades.Add(typeof(Marksmanship));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(R2A3BoY));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a4/Wedgeantilles-battleofyavin.png";

                PilotNameCanonical = "wedgeantilles-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WedgeAntillesBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddWedgeAntillesAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddWedgeAntillesAbility;
        }

        public void AddWedgeAntillesAbility()
        {
            foreach (GenericShip anotherFriednlyShip in HostShip.Owner.Ships.Values)
            {
                if (anotherFriednlyShip.ShipId == HostShip.ShipId) continue;

                ShotInfo shotInfo = new ShotInfo(Combat.Defender, anotherFriednlyShip, Combat.Defender.PrimaryWeapons);
                if (shotInfo.InArc)
                {
                    WedgeAntillesCondition condition = new WedgeAntillesCondition(Combat.Defender, HostShip);
                    Combat.Defender.Tokens.AssignCondition(condition);

                    return;
                }
            }
        }
    }
}