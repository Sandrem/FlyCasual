using Ship;
using Ship.SheathipedeShuttle;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;

namespace UpgradesList
{
    public class PhantomII : GenericUpgrade
    {
        public PhantomII() : base()
        {
            Type = UpgradeType.Title;
            Name = "Phantom II";
            Cost = 0;

            isUnique = true;

            UpgradeAbilities.Add(new PhantomIITitleAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is SheathipedeShuttle;
        }
    }
}

namespace Abilities
{
    public class PhantomIITitleAbility : GenericAbility
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
            Messages.ShowInfo("Ability of docked Phantom II");
        }

    }
}
