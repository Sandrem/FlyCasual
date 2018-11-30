using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Arcs;

namespace UpgradesList.FirstEdition
{
    public class Phantom : GenericUpgrade
    {
        public Phantom() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Phantom",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.AttackShuttle.AttackShuttle)),
                abilityType: typeof(Abilities.FirstEdition.PhantomTitleAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            Phases.Events.OnCombatPhaseEnd_Triggers += RegisterExtraShotAbility;
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
            Phases.Events.OnCombatPhaseEnd_Triggers -= RegisterExtraShotAbility;
        }

        private void ToggleRearArc(bool isActive)
        {
            HostShip.Host.ArcsInfo.GetArc<ArcSpecialGhost>().ShotPermissions.CanShootPrimaryWeapon = isActive;
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
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform additional turret attack.",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.Host.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private bool IsTurretAttack(GenericShip target, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            GenericUpgrade upgradeWeapon = weapon as GenericUpgrade;
            if (upgradeWeapon != null && upgradeWeapon.HasType(UpgradeType.Turret))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed from Turret");
            }

            return result;
        }

    }
}