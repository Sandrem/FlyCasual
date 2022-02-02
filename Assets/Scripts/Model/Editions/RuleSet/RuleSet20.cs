namespace Editions.RuleSets
{
    public class RuleSet20: RuleSet
    {
        public override string Name => "2.0";
        public override bool AllowTractoringOnObstacle => true;
        public override int PrimaryWeaponMinRange => 1;

        public override bool HasAttackRangeBonus(int range)
        {
            return range <= 1;
        }

        public override bool PreventDiceModification(int range)
        {
            return false;
        }
    }
}
