using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class GenericReinforceAction : GenericAction
    {

        public GenericReinforceAction()
        {
            Name = EffectName = "Reinforce (Generic)";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ApplyEvade();
            callBack();
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccesses = Combat.DiceRollAttack.Successes;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccesses > defenceSuccesses)
                {
                    result = 110;
                }
            }

            return result;
        }

    }

}
