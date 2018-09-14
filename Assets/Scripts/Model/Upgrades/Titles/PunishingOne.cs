using Ship;
using Ship.JumpMaster5000;
using Upgrade;
using Abilities;
using RuleSets;
using Abilities.SecondEdition;
using System.Collections.Generic;

namespace UpgradesList
{
    public class PunishingOne : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public PunishingOne() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Punishing One";
            Cost = 12;

            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Astromech)
            };
            UpgradeAbilities.Add(new PunishingOneAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 8;
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Crew
            };
            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new PunishingOneAbilitySe());

            SEImageNumber = 152;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is JumpMaster5000;
        }
    }
}

namespace Abilities
{
    public class PunishingOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ChangeFirepowerBy(1);
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeFirepowerBy(-1);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PunishingOneAbilitySe : GenericAbility
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
            if (HostShip.ArcInfo.GetArc<Arcs.ArcMobile>().Facing != Arcs.ArcFacing.Forward) count--;
        }
    }
}
