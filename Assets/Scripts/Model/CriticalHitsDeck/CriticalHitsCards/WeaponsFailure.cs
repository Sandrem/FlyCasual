using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class WeaponsFailure : GenericCriticalHit
    {
        public WeaponsFailure()
        {
            Name = "Weapons Failure";
            Type = CriticalCardType.Ship;
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGotNumberOfPrimaryWeaponAttackDices += ReduceNumberOfAttackDices;
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.AssignToken(new Tokens.WeaponsFailureCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Number of attack dices is restored");

            host.RemoveToken(typeof(Tokens.WeaponsFailureCritToken));
            host.AfterGotNumberOfPrimaryWeaponAttackDices -= ReduceNumberOfAttackDices;
            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void ReduceNumberOfAttackDices(ref int value)
        {
            Messages.ShowInfo("Weapons Failure: Number of attack dices is reduced");

            value--;
        }

    }

}