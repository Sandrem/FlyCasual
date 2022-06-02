using Actions;
using ActionsList;
using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class WartimeLoadout : GenericUpgrade
    {
        public WartimeLoadout() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Wartime Loadout",
                types: new List<UpgradeType> { UpgradeType.Configuration, UpgradeType.Modification },
                cost: 10,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.BTANR2YWing.BTANR2YWing)),
                addShields: 2,
                isStandardazed: true,
                addActions: new List<ActionInfo>()
                {
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(ReloadAction))
                },
                addSlots: new List<UpgradeSlot>()
                {
                    new UpgradeSlot(UpgradeType.Missile),
                    new UpgradeSlot(UpgradeType.Torpedo)
                },
                abilityType: typeof(Abilities.SecondEdition.WartimeLoadoutAbility)
            );
            
            ImageUrl = "https://i.imgur.com/Qe0Owij.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WartimeLoadoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            IntuitiveInterfaceAbility oldAbility = (IntuitiveInterfaceAbility)HostShip.ShipAbilities.First(n => n.GetType() == typeof(IntuitiveInterfaceAbility));
            oldAbility.DeactivateAbility();
            HostShip.ShipAbilities.Remove(oldAbility);

            GenericAbility ability = new DevastatingBarrageAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipAbilities.RemoveAll(n => n.GetType() == typeof(DevastatingBarrageAbility));

            GenericAbility ability = new IntuitiveInterfaceAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }
    }

    public class DevastatingBarrageAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += MakeCritsUncancellable;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= MakeCritsUncancellable;
        }

        private void MakeCritsUncancellable()
        {
            if ((Combat.ChosenWeapon.WeaponType == WeaponTypes.Torpedo || Combat.ChosenWeapon.WeaponType == WeaponTypes.Missile)
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Front))
            {
                foreach (Die die in Combat.DiceRollAttack.DiceList)
                {
                    if (die.Side == DieSide.Crit) die.IsUncancelable = true;
                }
            }
        }
    }
}