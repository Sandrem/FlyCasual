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
            Types.Add(UpgradeType.Title);
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
            ToggleRearArc(true);
            Phases.OnCombatPhaseEnd += RegisterExtraShotAbility;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            ToggleRearArc(false);
            Phases.OnCombatPhaseEnd -= RegisterExtraShotAbility;
        }

        private void ToggleRearArc(bool isActive)
        {
            HostShip.Host.ArcInfo.GetRearArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        private void RegisterExtraShotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, ExtraShotWithTurret);
        }

        private void ExtraShotWithTurret(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.Host.PilotName + " can perform second attack\nfrom Turret");
            Combat.StartAdditionalAttack(HostShip.Host, Triggers.FinishTrigger, IsTurretAttack);
        }

        private bool IsTurretAttack(GenericShip target, IShipWeapon weapon)
        {
            bool result = false;

            GenericUpgrade upgradeWeapon = weapon as GenericUpgrade;
            if (upgradeWeapon != null && upgradeWeapon.hasType(UpgradeType.Turret))
            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from Turret");
            }

            return result;
        }

    }
}
