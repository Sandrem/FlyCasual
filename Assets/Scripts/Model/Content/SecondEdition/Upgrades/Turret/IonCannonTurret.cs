using Actions;
using ActionsList;
using Arcs;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonCannonTurret : GenericSpecialWeapon
    {
        public IonCannonTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon Turret",
                UpgradeType.Turret,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.SingleTurret
                ),
                addArc: new ShipArcInfo(ArcType.SingleTurret),
                addAction: new ActionInfo(typeof(RotateArcAction)),
                abilityType: typeof(Abilities.SecondEdition.IonDamageAbilityTurret),
                seImageNumber: 32
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonDamageAbilityTurret : Abilities.SecondEdition.IonDamageAbility
    {
        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new RotateArcAction(), HostUpgrade);
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(RotateArcAction), HostUpgrade);
        }
    }
}