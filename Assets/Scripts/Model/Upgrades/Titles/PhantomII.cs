using Ship;
using Ship.SheathipedeShuttle;
using Upgrade;
using System.Linq;
using System;
using UnityEngine;
using Abilities;
using System.Collections.Generic;

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
            ToggleRearArc(true);
            Phases.OnActivationPhaseEnd += RegisterFreeCoordinateAbility;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            ToggleRearArc(false);
            Phases.OnActivationPhaseEnd -= RegisterFreeCoordinateAbility;
        }

        private void ToggleRearArc(bool isActive)
        {
            HostShip.Host.ArcInfo.GetRearArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        private void RegisterFreeCoordinateAbility()
        {
            if (HostShip.Host.Owner.Ships.Count > 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseEnd, FreeCoordinateAction);
            }
        }

        private void FreeCoordinateAction(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.CoordinateAction() };

            HostShip.Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

    }
}
