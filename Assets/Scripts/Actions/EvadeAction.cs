using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{

    public class EvadeAction : GenericAction
    {

        public EvadeAction() {
            Name = EffectName = "Evade";
        }

        public override void ActionEffect()
        {
            Game.Dices.ApplyEvade(Game.Combat.CurentDiceRoll);
            Debug.Log(typeof(Tokens.EvadeToken));
            Game.Selection.ActiveShip.SpendToken(typeof(Tokens.EvadeToken));
        }

        public override void ActionTake()
        {
            Game.Selection.ThisShip.AvailableActionEffects.Add(this);
            Game.Selection.ThisShip.AddToken(new Tokens.EvadeToken());
        }

    }

}
