using System;

namespace Editions.RuleSets
{
    public abstract class RuleSet
    {
        public abstract string Name { get; }
        public abstract bool AllowTractoringOnObstacle { get; }
        public abstract int PrimaryWeaponMinRange { get; }
        public abstract bool HasAttackRangeBonus(int range);
        public abstract bool PreventDiceModification(int range);
    }
}
