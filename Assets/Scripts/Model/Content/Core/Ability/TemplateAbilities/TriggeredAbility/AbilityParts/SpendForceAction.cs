namespace Abilities
{
    public class SpendForceAction : AbilityPart
    {
        private GenericAbility Ability;

        public SpendForceAction()
        {
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            Ability.HostShip.State.SpendForce(1, Triggers.FinishTrigger);
        }
    }
}
