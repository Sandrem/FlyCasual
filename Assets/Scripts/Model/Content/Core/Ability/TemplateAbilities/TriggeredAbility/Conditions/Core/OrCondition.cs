namespace Abilities
{
    public class OrCondition : Condition
    {
        public Condition[] Constions { get; }

        public OrCondition(params Condition[] constions)
        {
            Constions = constions;
        }

        public override bool Passed(ConditionArgs args)
        {
            foreach (Condition condition in Constions)
            {
                if (condition.Passed(args)) return true;
            }

            return false;
        }
    }
}
