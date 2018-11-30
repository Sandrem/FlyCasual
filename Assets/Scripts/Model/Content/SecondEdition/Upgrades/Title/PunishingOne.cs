using Ship;
using Upgrade;
using System.Collections.Generic;
using Arcs;

namespace UpgradesList.SecondEdition
{
    public class PunishingOne : GenericUpgradeSlotUpgrade
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
                abilityType: typeof(Abilities.SecondEdition.PunishingOneAbility),
                seImageNumber: 152
            );

            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Crew
            };
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PunishingOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ChangeFirepowerBy(1);
            HostShip.AfterGotNumberOfAttackDice += CheckWeakArc;
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeFirepowerBy(-1);
            HostShip.AfterGotNumberOfAttackDice -= CheckWeakArc;
        }

        private void CheckWeakArc(ref int count)
        {
            if (HostShip.ArcsInfo.GetArc<Arcs.ArcMobile>().Facing != ArcFacing.Forward) count--;
        }
    }
}