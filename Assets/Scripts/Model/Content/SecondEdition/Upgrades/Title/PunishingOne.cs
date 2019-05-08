using Ship;
using Upgrade;
using System.Collections.Generic;
using Arcs;

namespace UpgradesList.SecondEdition
{
    public class PunishingOne : GenericUpgrade
    {
        public PunishingOne() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Punishing One",
                UpgradeType.Title,
                cost: 8,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.JumpMaster5000.JumpMaster5000)),
                addSlot: new UpgradeSlot(UpgradeType.Astromech),
                forbidSlot: UpgradeType.Crew,
                abilityType: typeof(Abilities.SecondEdition.PunishingOneAbility),
                seImageNumber: 152
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PunishingOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckArcBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckArcBonus;
        }

        private void CheckArcBonus(ref int count)
        {
            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Front)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
            )
            {
                Messages.ShowInfo("Punishing One is attacking a ship with its primary weapon and gains +1 attack die");
                count++;
            }
        }
    }
}