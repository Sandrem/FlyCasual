namespace Abilities
{
    public class AndCondition : Condition
    {
        public Condition[] Constions { get; }

        public AndCondition(params Condition[] constions)
        {
            Constions = constions;
        }

        public override bool Passed(ConditionArgs args)
        {
            foreach (Condition condition in Constions)
            {
                if (!condition.Passed(args)) return false;
            }

            return true;
        }
    }
}
