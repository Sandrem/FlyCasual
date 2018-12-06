using Ship;
using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Arcs;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class PhantomII : GenericUpgrade
    {
        public PhantomII() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Phantom II",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.SheathipedeClassShuttle.SheathipedeClassShuttle)),
                abilityType: typeof(Abilities.FirstEdition.PhantomIITitleAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            Phases.Events.OnActivationPhaseEnd_Triggers += RegisterFreeCoordinateAbility;
            dockingHost.OnShipIsDestroyed += DeactivateFreeCoordinate;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            ToggleRearArc(false);
            DeactivateFreeCoordinate(dockingHost, false);
            HostShip.OnShipIsDestroyed -= DeactivateFreeCoordinate;
        }

        private void DeactivateFreeCoordinate(GenericShip host, bool isFled)
        {
            Phases.Events.OnActivationPhaseEnd_Triggers -= RegisterFreeCoordinateAbility;
        }

        private void ToggleRearArc(bool isActive)
        {
            HostShip.Host.ArcsInfo.GetArc<ArcSpecialGhost>().ShotPermissions.CanShootPrimaryWeapon = isActive;
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
            List<GenericAction> actions = new List<GenericAction>() { new CoordinateAction() };

            HostShip.Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

    }
}