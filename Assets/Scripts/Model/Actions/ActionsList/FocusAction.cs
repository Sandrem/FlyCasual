using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class FocusAction: GenericAction
    {

        public FocusAction() {
            Name = EffectName = "Focus";
        }

        public override void ActionEffect()
        {
            Dices.ApplyFocus(Combat.CurentDiceRoll);
            Game.Selection.ActiveShip.SpendToken(typeof(Tokens.FocusToken));
        }

        public override void ActionTake()
        {
            Game.Selection.ThisShip.AssignToken(new Tokens.FocusToken());
        }

    }

}
