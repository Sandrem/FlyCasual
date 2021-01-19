using BoardTools;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SensorBuoySuite : GenericUpgrade
    {
        public SensorBuoySuite() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sensor Buoy Suite",
                UpgradeType.Tech,
                cost: 4,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large)
                ),
                abilityType: typeof(Abilities.SecondEdition.SensorBuoySuiteAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7d/19/7d199d6b-808a-47d2-9aa1-1fc3432e7d3f/swz69_sensor-buoy-suite_card.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorBuoySuiteAbility : GenericAbility
    {
        private int remotesPlaced;

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterAbility;
            HostShip.OnCombatActivation += CheckLockTargets;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterAbility;
            HostShip.OnCombatActivation -= CheckLockTargets;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupStart, PlaceRemote);
        }

        private void PlaceRemote(object sender, EventArgs e)
        {
            RemoteSetupSubPhase subPhase = Phases.StartTemporarySubPhaseNew<RemoteSetupSubPhase>("Remote setup", TryFinish);
            
            subPhase.PrepareRemoteSetup(typeof(SensorBuoy), HostShip.Owner);
            subPhase.MinBoardEdgeDistance = Board.BoardIntoWorld(2 * Board.RANGE_1);

            subPhase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            string remoteNumber = (remotesPlaced == 0) ? "first" : "second";
            subPhase.DescriptionLong = $"Place {remoteNumber} buoy remote";
            subPhase.ImageSource = HostUpgrade;

            subPhase.Start();
        }

        private void TryFinish()
        {
            remotesPlaced++;

            if (remotesPlaced < 2)
            {
                PlaceRemote(null, null);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void CheckLockTargets(GenericShip ship)
        {
            if (HasLockTargets())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToSelectLockTarget);
            }
        }

        private bool HasLockTargets()
        {
            foreach (SensorBuoy buoy in HostShip.Owner.Units.Values.Where(n => n is SensorBuoy))
            {
                foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
                {
                    DistanceInfo distInfo = new DistanceInfo(buoy, enemyShip);
                    if (distInfo.Range <= 1) return true;
                }
            }

            return false;
        }

        private void AskToSelectLockTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GetLockIgnoringRange,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "You may acquire a lock on a ship at range 0-1 of a friendly sensor buoy, ignoring range restrictions",
                imageSource: HostUpgrade
            );
        }

        private void GetLockIgnoringRange()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.AcquireTargetLock(
                HostShip,
                TargetShip,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger,
                ignoreRange: true
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            foreach (SensorBuoy buoy in HostShip.Owner.Units.Values.Where(n => n is SensorBuoy))
            {
                DistanceInfo distInfo = new DistanceInfo(buoy, ship);
                if (distInfo.Range <= 1) return true;
            }

            return false;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}