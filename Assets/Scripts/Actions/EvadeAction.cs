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
            Game.Selection.ActiveShip.SpendToken(typeof(Tokens.EvadeToken));
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Game.Combat.AttackStep == CombatStep.Defence) result = true;
            return result;
        }

        public override void ActionTake()
        {
            Game.Selection.ThisShip.AssignToken(new Tokens.EvadeToken());
        }

    }

}
