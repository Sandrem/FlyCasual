using Arcs;
using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HeavyLaserCannon : GenericSpecialWeapon
    {
        public HeavyLaserCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Heavy Laser Cannon",
                UpgradeType.Cannon,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    arc: ArcType.Bullseye
                ),
                abilityType: typeof(Abilities.SecondEdition.HeavyLaserCannonAbility),
                seImageNumber: 27
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HeavyLaserCannonAbility : Abilities.FirstEdition.HeavyLaserCannonAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterAttackDiceModification += ChangeCritsToHits;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterAttackDiceModification -= ChangeCritsToHits;
        }

        private void ChangeCritsToHits()
        {
            if (Combat.ChosenWeapon is UpgradesList.SecondEdition.HeavyLaserCannon)
            {
                Combat.DiceRollAttack.ChangeAll(DieSide.Crit, DieSide.Success);
            }
        }
    }
}