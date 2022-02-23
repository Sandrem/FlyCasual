using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class AutopilotDrone : EscapeCraft
        {
            public AutopilotDrone() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Autopilot Drone",
                    "Set to Blow",
                    Faction.Scum,
                    1,
                    2,
                    0,
                    isLimited: true,
                    charges: 3,
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    seImageNumber: 229
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ShipAbilities.Add(new Abilities.SecondEdition.AutopilotDroneAbility());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AutopilotDroneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterLoseCharge;
            Phases.Events.OnActivationPhaseEnd_Triggers += CheckDestruction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterLoseCharge;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CheckDestruction;
        }

        private void RegisterLoseCharge(GenericShip ship)
        {
            if (ship.DockingHost == null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, LoseCharge);
            }
        }

        private void LoseCharge(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " loses 1 charge");
            HostShip.RemoveCharge(Triggers.FinishTrigger);
        }

        private void CheckDestruction()
        {
            if (HostShip.State.Charges == 0) RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseEnd, DestroyThisShip);
        }

        private void DestroyThisShip(object sender, System.EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Values)
            {
                if (ship.ShipId == HostShip.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 2) sufferedShips.Add(ship);
            }

            Messages.ShowInfo("Autopilot Drone is destroyed");
            HostShip.DestroyShipForced(delegate { DealDamageToShips(sufferedShips, 1, true, Triggers.FinishTrigger); });
        }        
    }
}