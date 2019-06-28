using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Autoblaster : GenericSpecialWeapon
    {
        public Autoblaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Autoblaster",
                UpgradeType.Cannon,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 1
                ),
                abilityType: typeof(Abilities.FirstEdition.AutoblasterAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AutoblasterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += RegisterAutoblasterEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= RegisterAutoblasterEffect;
        }

        private void RegisterAutoblasterEffect()
        {
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;

                foreach (Die die in Combat.DiceRollAttack.DiceList)
                {
                    if (die.Side == DieSide.Success)
                    {
                        die.IsUncancelable = true;
                    }
                }
            }
        }
    }
}