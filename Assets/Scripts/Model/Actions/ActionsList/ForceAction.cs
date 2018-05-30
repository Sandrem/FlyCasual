namespace ActionsList
{

    public class ForceAction : GenericAction
    {

        public ForceAction()
        {
            Name = EffectName = "Force";

            IsTurnsOneFocusIntoSuccess = true;
            CanBeUsedFewTimes = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            Host.Force--;
            callBack();
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.DiceRollDefence.Focuses;
                    if (defenceFocuses > 0)
                    {
                        result = (defenceFocuses > 1) ? 35 : 45;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0)
                {
                    result = (attackFocuses > 1) ? 35 : 45;
                }
            }

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            return Host.Force > 0;
        }

    }

}