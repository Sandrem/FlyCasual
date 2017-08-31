using Ship;

namespace Upgrade
{
    public class GenericActionBarUpgrade<T> : GenericUpgrade where T: ActionsList.GenericAction, new()
    {
        protected ActionsList.GenericAction Action { get; set; }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.AfterGenerateAvailableActionsList += AddAction;
        }

        private void AddAction(Ship.GenericShip host)
        {
            host.AddAvailableAction(new T());
        }
    }
}
