namespace Abilities
{
    public class SpendPilotChargeAction : AbilityPart
    {
        private GenericAbility Ability;
        private AbilityPart Next = null;

        public SpendPilotChargeAction(AbilityPart next = null)
        {
            Next = next;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            Ability.HostShip.SpendCharge();
            FinishAbilityPart();
        }
        private void FinishAbilityPart()
        {
            if (Next != null)
            {
                Next.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
