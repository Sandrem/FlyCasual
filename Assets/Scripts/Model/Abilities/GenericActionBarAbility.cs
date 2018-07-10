using ActionsList;
using Ship;

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

        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        private void AddAction(GenericShip host)
        {            
            var alreadyHasAction = host.PrintedActions.Find(action => 
                action is T && action.IsRed == IsRed
                && (LinkedAction == null || action.LinkedRedAction == null 
                    || (action.LinkedRedAction.GetType() == LinkedAction.GetType() && action.LinkedRedAction.IsRed == LinkedAction.IsRed))
            );
            if (alreadyHasAction == null)
            {
                var action = new T();
                action.IsRed = IsRed;
                action.LinkedRedAction = LinkedAction;
                host.AddAvailableAction(action);
            }
        }

    }
}
