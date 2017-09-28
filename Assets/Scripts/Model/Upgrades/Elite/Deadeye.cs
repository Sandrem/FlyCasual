using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using Tokens;
using System.Linq;

namespace UpgradesList
{
    public class Deadeye : GenericUpgrade
    {
        public Deadeye() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Deadeye";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            Host.OnGenerateAvailableAttackPaymentList += AddFocusTokenAsPayment;
        }

        private void AddFocusTokenAsPayment(List<GenericToken> waysToPay)
        {
            if (waysToPay.Find(n => n.GetType() == typeof(BlueTargetLockToken)) != null)
            {
                GenericToken focus = null;
                if (Host.HasToken(typeof(FocusToken))) focus = Host.GetToken(typeof(FocusToken));
                if (focus != null) waysToPay.Add(focus);
            }
        }
    }
}
