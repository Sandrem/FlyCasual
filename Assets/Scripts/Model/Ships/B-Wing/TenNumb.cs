using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace Ship
{
    namespace BWing
    {
        public class TenNumb : BWing
        {
            public TenNumb() : base()
            {
                PilotName = "Ten Numb";
                PilotSkill = 8;
                Cost = 31;
                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
 
                PilotAbilities.Add(new Abilities.TenNumbAbiliity());
                SkinName = "Blue";
            }
        }
    }
}
 
namespace Abilities
{
    public class TenNumbAbiliity : GenericAbility
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
