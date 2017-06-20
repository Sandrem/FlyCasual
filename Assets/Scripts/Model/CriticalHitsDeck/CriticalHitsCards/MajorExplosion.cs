using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class MajorExplosion : GenericCriticalHit
    {
        public MajorExplosion()
        {
            Name = "Major Explosion";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            Game.UI.AddTestLogEntry("Roll 1 attack die. On a Hit result, suffer 1 critical damage.");
            RollForDamage(host);
        }

        private void RollForDamage(Ship.GenericShip host)
        {
            Selection.ActiveShip = host;
            Phases.StartTemporarySubPhase("Major Explosion", typeof(SubPhases.MajorExplosionCheckSubPhase));
        }

    }

}

namespace SubPhases
{

    public class MajorExplosionCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            dicesType = "attack";
            dicesCount = 1;

            checkResults = CheckResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            if (diceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Game.UI.ShowError("Major Explosion: Suffer 1 additional critical damage");
                Game.UI.AddTestLogEntry("Major Explosion: Suffer 1 additional critical damage");

                Game.StartCoroutine(DealDamage());
            }

            base.CheckResults(diceRoll);
        }

        private IEnumerator DealDamage()
        {
            Triggers.AddTrigger("Draw faceup damage card", TriggerTypes.OnDamageCardIsDealt, CriticalHitsDeck.DrawCrit, Selection.ActiveShip, Selection.ActiveShip.Owner.PlayerNo);
            yield return Triggers.ResolveAllTriggers(TriggerTypes.OnDamageCardIsDealt);
        }

    }

}