using Ship;
using UnityEngine;
using System.Linq;
using ActionsList;

namespace Upgrade
{
    public class GenericActionBarUpgrade<T> : GenericUpgrade where T: GenericAction, new()
    {
        protected GenericAction Action { get; set; }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.AfterGenerateAvailableActionsList += AddAction;
        }

        private void AddAction(GenericShip host)
        {
            if (!isDiscarded)
            {
                var alreadyHasAction = host.PrintedActions.Find(n => n.GetType() == typeof(T));
                if (alreadyHasAction == null) host.AddAvailableAction(new T());
            }
        }
    }
}
