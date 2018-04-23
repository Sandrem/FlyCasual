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
            Phases.OnCombatPhaseEnd_Triggers += RegisterExtraShotAbility;
            dockingHost.OnShipIsDestroyed += DeactivateSecondAttack;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            ToggleRearArc(false);
            DeactivateSecondAttack(dockingHost, false);
            HostShip.OnShipIsDestroyed -= DeactivateSecondAttack;
        }

        private void DeactivateSecondAttack(GenericShip host, bool isFled)
        {
            Phases.OnCombatPhaseEnd_Triggers -= RegisterExtraShotAbility;
        }

        private void ToggleRearArc(bool isActive)
        {
            HostShip.Host.ArcInfo.GetRearArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        private void RegisterExtraShotAbility()
        {
            if (!HostShip.Host.IsCannotAttackSecondTime)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, ExtraShotWithTurret);
            }
        }

        private void ExtraShotWithTurret(object sender, System.EventArgs e)
        {
            if (!HostShip.Host.IsCannotAttackSecondTime)
            {
                HostShip.Host.IsCannotAttackSecondTime = true;

                Combat.StartAdditionalAttack(
                    HostShip.Host,
                    Triggers.FinishTrigger,
                    IsTurretAttack,
                    HostUpgrade.Name,
                    "You may perfrom additional turret attack.",
                    HostUpgrade.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.Host.PilotName));
                Triggers.FinishTrigger();
            }            
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
