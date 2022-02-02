namespace Editions.RuleSets
{
    public class RuleSet25 : RuleSet
    {
        public override string Name => "2.5";
        public override bool AllowTractoringOnObstacle => false;
        public override int PrimaryWeaponMinRange => 0;

        public override bool HasAttackRangeBonus(int range)
        {
            return range == 1;
        }

        public override bool PreventDiceModification(int range)
        {
            return range == 0;
        }
    }
}
