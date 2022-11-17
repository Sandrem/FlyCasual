namespace Abilities
{
    public class HasPilotChargesAbility : Condition
    {
        private int PilotChargesCount;

        public HasPilotChargesAbility(int count)
        {
            PilotChargesCount = count;
        }

        public override bool Passed(ConditionArgs args)
        {
            return args.ShipAbilityHost.State.Charges >= PilotChargesCount;
        }
    }
}
