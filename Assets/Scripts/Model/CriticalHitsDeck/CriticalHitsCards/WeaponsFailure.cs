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
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/7/76/Swx36-weapons-failure.png";
            CancelDiceResults.Add(DiceSide.Success);
            CancelDiceResults.Add(DiceSide.Crit);
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("When attacking, roll 1 fewer attack die");
            Game.UI.AddTestLogEntry("When attacking, roll 1 fewer attack die");
            host.AssignToken(new Tokens.WeaponsFailureCritToken());

            host.AfterGotNumberOfPrimaryWeaponAttackDices += ReduceNumberOfAttackDices;

            host.AfterAvailableActionListIsBuilt += AddCancelCritAction;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Number of attack dices is restored");
            Game.UI.AddTestLogEntry("Number of attack dices is restored");
            host.RemoveToken(typeof(Tokens.WeaponsFailureCritToken));

            host.AfterGetPilotSkill -= ReduceNumberOfAttackDices;

            host.AfterAvailableActionListIsBuilt -= AddCancelCritAction;
        }

        private void ReduceNumberOfAttackDices(ref int value)
        {
            Game.UI.ShowError("Weapons Failure: Number of attack dices is reduced");
            Game.UI.AddTestLogEntry("Weapons Failure: Number of attack dices is reduced");
            value--;
        }

    }

}