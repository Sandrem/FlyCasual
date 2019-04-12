using Editions;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace ActionsList
{

    public class EvadeAction : GenericAction
    {

        public EvadeAction()
        {
            Name = DiceModificationName = "Evade";
            CanBeUsedFewTimes = (Edition.Current is Editions.SecondEdition);

            TokensSpend.Add(typeof(EvadeToken));
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyEvade();
            Selection.ActiveShip.Tokens.SpendToken(typeof(EvadeToken), callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Edition.Current is Editions.SecondEdition)
                {
                     if (Combat.CurrentDiceRoll.Count != 0) result = true;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    result = (attackSuccessesCancelable - defenceSuccesses == 1) ? 70 : 20;
                }
            }

            if (Edition.Current is Editions.SecondEdition && Combat.DiceRollDefence.Failures == 0) return 0;

            return result;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(EvadeToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            // Increase the chance to evade if the ship has only 1 hull and has enemies targeting it.
            if (Selection.ThisShip.State.HullCurrent == 1 && Selection.ThisShip.State.Agility >= 1 && ActionsHolder.CountEnemiesTargeting(HostShip, 0) > 0)
            {
                result += 80;
            }

            return result;
        }

    }

}
