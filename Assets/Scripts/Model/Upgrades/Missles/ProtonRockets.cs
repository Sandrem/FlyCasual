using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using Upgrade;

namespace UpgradesList
{
    public class ProtonRockets : GenericSecondaryWeapon
    {
        public ProtonRockets() : base()
        {
            Type = UpgradeType.Missile;

            Name = "Proton Rockets";
            Cost = 3;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 2;

            RequiresFocusToShoot = true;

            IsDiscardedForShot = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackStartAsAttacker += AddAgilityToAttackValue;
        }

        private void AddAgilityToAttackValue()
        {
            int attackModifier = 3;
            if (Host.Agility < 3)
            {
                attackModifier = Host.Agility;
            }
            AttackValue = 2 + attackModifier;
        }
    }
}
