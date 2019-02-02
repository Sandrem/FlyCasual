using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class DorsalTurret : GenericSpecialWeapon
    {
        public DorsalTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dorsal Turret",
                UpgradeType.Turret,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 2,
                    canShootOutsideArc: true
                ),
                abilityType: typeof(Abilities.FirstEdition.DorsalTurretAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DorsalTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += AddDiceAtRangeOne;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= AddDiceAtRangeOne;
        }

        private void AddDiceAtRangeOne(ref int diceCount)
        {
            if (Combat.ChosenWeapon == HostUpgrade && Combat.ShotInfo.Range == 1)
            {
                diceCount++;
            }
        }
    }
}