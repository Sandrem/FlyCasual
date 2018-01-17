using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Ship;

namespace UpgradesList
{

    public class Juke : GenericUpgrade
    {

        public Juke() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Juke";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
            RegisterAbilityTrigger(TriggerTypes., ShowDecision);
            Host.OnImmediatelyAfterRolling += ChangeCritsToHits;
            Console.Write("Juke attached");
        }

        private void ChangeCritsToHits(DiceRoll diceroll)
        {
            Console.Write("Juke executed");
            diceroll.ChangeAll(DieSide.Blank, DieSide.Crit);
            diceroll.ChangeAll(DieSide.Focus, DieSide.Crit);
            diceroll.ChangeAll(DieSide.Success, DieSide.Crit);
        }
    }
}