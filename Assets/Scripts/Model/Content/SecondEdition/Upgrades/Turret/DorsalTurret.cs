using Actions;
using ActionsList;
using Arcs;
using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DorsalTurret : GenericSpecialWeapon
    {
        public DorsalTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dorsal Turret",
                UpgradeType.Turret,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.SingleTurret
                ),
                addAction: new ActionInfo(typeof(RotateArcAction)),
                abilityType: typeof(Abilities.SecondEdition.DorsalTurretAbility),
                seImageNumber: 31
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DorsalTurretAbility : GenericAbility
    {
        public override void ActivateAbility() { }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new RotateArcAction(), HostUpgrade);
        }

        public override void DeactivateAbility() { }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(RotateArcAction), HostUpgrade);
        }

    }
}