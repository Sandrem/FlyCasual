using Abilities;
using Upgrade;
using UpgradesList;

namespace UpgradesList
{

    public class HeavyLaserCannon : GenericSecondaryWeapon
    {
        public HeavyLaserCannon() : base()
        {
            Types.Add(UpgradeType.Cannon);

            Name = "Heavy Laser Cannon";
            Cost = 7;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            UpgradeAbilities.Add(new HeavyLaserCannonAbility());
        }

    }
}

namespace Abilities
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
            if (Combat.ChosenWeapon is HeavyLaserCannon)
            {
                diceroll.ChangeAll(DieSide.Crit, DieSide.Success);
            }
        }
    }
}