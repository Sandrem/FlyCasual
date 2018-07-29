using Abilities;
using Upgrade;
using UpgradesList;
using RuleSets;

namespace UpgradesList
{

    public class HeavyLaserCannon : GenericSecondaryWeapon, ISecondEditionUpgrade
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

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;

            ArcRestrictions.Add(Arcs.ArcTypes.Bullseye);
            UpgradeAbilities.RemoveAll(a => a is HeavyLaserCannonAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.HeavyLaserCannonAbilitySE());
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

namespace Abilities.SecondEdition
{
    public class HeavyLaserCannonAbilitySE : HeavyLaserCannonAbility
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
            if (Combat.ChosenWeapon is HeavyLaserCannon)
            {
                Combat.DiceRollAttack.ChangeAll(DieSide.Crit, DieSide.Success);
            }
        }
    }
}