namespace Editions.RuleSets
{
    public abstract class RuleSet
    {
        public abstract string Name { get; }
        public abstract bool AllowTractoringOnObstacle { get; }
    }
}
