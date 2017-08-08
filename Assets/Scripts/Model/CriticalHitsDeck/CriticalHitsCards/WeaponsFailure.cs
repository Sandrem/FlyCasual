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
            ImageUrl = "http://i.imgur.com/nUj64yn.jpg";
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("When attacking, roll 1 fewer attack die");
            Game.UI.AddTestLogEntry("When attacking, roll 1 fewer attack die");
            Host.AssignToken(new Tokens.WeaponsFailureCritToken());

            Host.AfterGotNumberOfPrimaryWeaponAttackDices += ReduceNumberOfAttackDices;

            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Number of attack dices is restored");
            Game.UI.AddTestLogEntry("Number of attack dices is restored");
            host.RemoveToken(typeof(Tokens.WeaponsFailureCritToken));

            host.AfterGetPilotSkill -= ReduceNumberOfAttackDices;

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void ReduceNumberOfAttackDices(ref int value)
        {
            Game.UI.ShowError("Weapons Failure: Number of attack dices is reduced");
            Game.UI.AddTestLogEntry("Weapons Failure: Number of attack dices is reduced");
            value--;
        }

    }

}