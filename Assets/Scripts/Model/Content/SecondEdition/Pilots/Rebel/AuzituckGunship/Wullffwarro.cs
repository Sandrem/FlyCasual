using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wullffwarro",
                    "Wookiee Chief",
                    Faction.Rebel,
                    4,
                    5,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WullffwarroAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    seImageNumber: 31,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WullffwarroAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckWullffwarroAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckWullffwarroAbility;
        }

        private void CheckWullffwarroAbility(ref int value)
        {
            if (HostShip.Damage.IsDamaged)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is damaged, gains +1 attack die");
                value++;
            }
        }
    }
}
