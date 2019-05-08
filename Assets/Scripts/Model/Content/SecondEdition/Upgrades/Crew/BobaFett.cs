using Ship;
using Upgrade;
using System;
using SubPhases;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class BobaFett : GenericUpgrade
    {
        public BobaFett() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Boba Fett",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.BobaFettCrewAbility),
                seImageNumber: 129
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BobaFettCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += MoveToReserve;
            Phases.Events.OnSetupEnd += RegisterReturn;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= MoveToReserve;
            Phases.Events.OnSetupEnd -= RegisterReturn;
        }

        private void MoveToReserve()
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + "has been moved to the Reserve");
            Roster.MoveToReserve(HostShip);
        }

        private void RegisterReturn()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, SetupShip);
        }

        private void SetupShip(object sender, EventArgs e)
        {
            Roster.ReturnFromReserve(HostShip);

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                delegate {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has been placed");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = Direction.None;
            subphase.AbilityName = HostUpgrade.UpgradeInfo.Name;
            subphase.Description = "Place yourself at range 0 of an obstacle and beyond range 3 of any enemy ship.";
            subphase.ImageSource = HostUpgrade;
            subphase.SetupFilter = SetupFilter;

            subphase.Start();
        }

        private bool SetupFilter()
        {
            bool result = true;

            if (HostShip.Model.GetComponentInChildren<ObstaclesStayDetector>().OverlapedAsteroids.Count == 0)
            {
                Messages.ShowErrorToHuman("Boba Fett: Invalid location for this ship, the ship must be placed at range 0 of an asteroid");
                return false;
            }

            foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, enemyShip);
                if (distInfo.Range < 4)
                {
                    Messages.ShowErrorToHuman("Boba Fett: The range to the closest enemy is " + distInfo.Range + ", it must be beyond range 3");
                    return false;
                }
            }

            return result;
        }
    }
}