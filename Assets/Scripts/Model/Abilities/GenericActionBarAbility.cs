using ActionsList;
using Ship;

namespace Abilities
{
    public class GenericActionBarAbility<T> : GenericAbility where T : GenericAction, new()
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= AddAction;
        }

        private void AddAction(GenericShip host)
        {            
            var alreadyHasAction = host.PrintedActions.Find(n => n.GetType() == typeof(T));
            if (alreadyHasAction == null) host.AddAvailableAction(new T());            
        }
    }
}
