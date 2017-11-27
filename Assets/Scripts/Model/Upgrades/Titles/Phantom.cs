using Ship;
using Ship.AttackShuttle;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;

namespace UpgradesList
{
    public class Phantom : GenericUpgrade
    {
        public Phantom() : base()
        {
            Type = UpgradeType.Title;
            Name = "Phantom";
            Cost = 0;

            isUnique = true;

            IsHidden = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AttackShuttle;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnDocked += OnDocked;
            Host.OnUndocked += OnUndocked;
        }

        private void OnDocked(GenericShip dockingHost)
        {
            dockingHost.OnAttackStartAsAttacker += TestAbility;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            dockingHost.OnAttackStartAsAttacker -= TestAbility;
        }

        private void TestAbility()
        {
            Debug.Log("Ability of docked Phantom");
        }

    }
}
