using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class KazudaXiono : Fireball
        {
            public KazudaXiono() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kazuda Xiono",
                    "Best Pilot in the Galaxy",
                    Faction.Resistance,
                    4,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KazudaXionoAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/16/42/164202d5-47a5-44ea-a96b-49c795005ca8/swz63_a1_card_kazuda-xiono.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // TODO: MAY

    public class KazudaXionoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += CheckDefenseBonus;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckPrimaryWeaponAttackBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += CheckDefenseBonus;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckPrimaryWeaponAttackBonus;
        }

        private void CheckDefenseBonus(ref int count)
        {
            if (Combat.Attacker.State.Initiative > HostShip.Damage.DamageCards.Count)
            {
                count++;
                Messages.ShowInfo("Initiative of attacker is higher than the number of damage cards " + HostShip.PilotInfo.PilotName + " has, " + HostShip.PilotInfo.PilotName + " rolls +1 defense die");
            }
        }

        private void CheckPrimaryWeaponAttackBonus(ref int count)
        {
            if (Combat.Defender.State.Initiative > HostShip.Damage.DamageCards.Count)
            {
                count++;
                Messages.ShowInfo("Initiative of defender is higher than the number of damage cards " + HostShip.PilotInfo.PilotName + " has, " + HostShip.PilotInfo.PilotName + " rolls +1 attack die");
            }
        }
    }
}