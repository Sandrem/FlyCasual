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
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("When attacking, roll 1 fewer attack die");
            Game.UI.AddTestLogEntry("When attacking, roll 1 fewer attack die");

            host.AfterGotNumberOfPrimaryWeaponAttackDices += ReduceNumberOfAttackDices;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Number of attack dices is restored");
            Game.UI.AddTestLogEntry("Number of attack dices is restored");

            host.AfterGetPilotSkill -= ReduceNumberOfAttackDices;
        }

        private void ReduceNumberOfAttackDices(ref int value)
        {
            Game.UI.ShowError("Weapons Failure: Number of attack dices is reduced");
            Game.UI.AddTestLogEntry("Weapons Failure: Number of attack dices is reduced");
            value--;
        }

    }

}