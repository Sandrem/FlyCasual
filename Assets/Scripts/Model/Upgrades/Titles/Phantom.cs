using Ship;
using Ship.AttackShuttle;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;

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

            UpgradeAbilities.Add(new PhantomTitleAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AttackShuttle;
        }
    }
}

namespace Abilities
{
    public class PhantomTitleAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDocked += OnDocked;
            HostShip.OnUndocked += OnUndocked;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDocked -= OnDocked;
            HostShip.OnUndocked -= OnUndocked;
        }

        private void OnDocked(GenericShip dockingHost)
        {
            Phases.OnCombatPhaseEnd += TestAbility;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            Phases.OnCombatPhaseEnd -= TestAbility;
        }

        private void TestAbility()
        {
            Messages.ShowInfo("Ability of deocked Phantom");
        }

    }
}
