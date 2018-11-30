using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class TenNumb : BWing
        {
            public TenNumb() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ten Numb",
                    8,
                    31,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TenNumbAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Dark Blue";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TenNumbAbility : GenericAbility
    {
        // When attacking or defending, if you have at least 1 stress token,
        // you may reroll 1 of your dice.
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += RegisterTenNumbEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= RegisterTenNumbEffect;
        }

        private void RegisterTenNumbEffect()
        {
            foreach (Die die in Combat.DiceRollAttack.DiceList)
            {
                if (die.Side == DieSide.Crit)
                {
                    die.IsUncancelable = true;
                    return;
                }
            }
        }
    }
}