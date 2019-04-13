using Tokens;

namespace ActionsList
{

    public class ForceAction : GenericAction
    {

        public ForceAction()
        {
            Name = DiceModificationName = "Force";

            IsTurnsOneFocusIntoSuccess = true;
            CanBeUsedFewTimes = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            HostShip.State.Force--;
            callBack();
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
               if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.DiceRollDefence.Focuses;
                    int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                    if (numFocusTokens > 0 && defenceFocuses > 1)
                    {
                        // Multiple focus results on our defense roll and we have a Focus token.  Use it instead of the Force.
                        result = 0;
                    }
                    else if (defenceFocuses > 0)
                    {
                        // We don't have a focus token.  Better use the Force.
                        result = 45;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.Focuses;
                if (attackFocuses > 0)
                {
                    int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                    if(numFocusTokens > 0 && attackFocuses > 1)
                    {
                        // We have a focus token.  Use it instead of the Force.
                        result = 0;
                    }
                    else
                    {
                        result = 45;
                    }
                    
                }
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return HostShip.State.Force > 0 && Combat.CurrentDiceRoll.Focuses != 0;
        }

    }

}