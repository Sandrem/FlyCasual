using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class HeavyLaserCannon : GenericSpecialWeapon
    {
        public HeavyLaserCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Heavy Laser Cannon",
                UpgradeType.Cannon,
                cost: 7,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.FirstEdition.HeavyLaserCannonAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class HeavyLaserCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += ChangeCritsToHits;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= ChangeCritsToHits;
        }

        private void ChangeCritsToHits(DiceRoll diceroll)
        {
            if (Combat.ChosenWeapon is UpgradesList.FirstEdition.HeavyLaserCannon)
            {
                diceroll.ChangeAll(DieSide.Crit, DieSide.Success);
            }
        }
    }
}