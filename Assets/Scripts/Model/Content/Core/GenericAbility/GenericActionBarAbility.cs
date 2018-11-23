using ActionsList;

namespace Abilities
{
    public class GenericActionBarAbility<T> : GenericAbility where T : GenericAction, new()
    {
        protected bool IsRed;
        protected GenericAction LinkedAction;

        public GenericActionBarAbility(bool isRed = false, GenericAction linkedAction = null)
        {
            IsRed = isRed;
            LinkedAction = linkedAction;
        }

        public override void ActivateAbility(){}
        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new T(), HostUpgrade);
        }

        public override void DeactivateAbility(){}
        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(T), HostUpgrade);
        }
    }
}
