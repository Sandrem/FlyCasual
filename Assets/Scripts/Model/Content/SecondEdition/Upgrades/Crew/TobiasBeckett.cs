using Ship;
using Upgrade;
using System;
using SubPhases;
using BoardTools;
using Obstacles;
using UnityEngine;
using Content;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class TobiasBeckett : GenericUpgrade
    {
        public TobiasBeckett() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tobias Beckett",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.TobiasBeckettAbility),
                seImageNumber: 160,
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(342, 6)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TobiasBeckettAbility : GenericAbility
    {
        public GenericObstacle ChosenObstacle { get; private set; }

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterOwnAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterOwnAbility;
        }

        private void RegisterOwnAbility()
        {
            // Skip if ship controlled by AI
            if (Roster.GetPlayer(HostShip.Owner.PlayerNo) is Players.AggressorAiPlayer) return;

            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, SelectObstacle);
        }

        private void SelectObstacle(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectObstacleSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectObstacleSubPhase>(
                HostUpgrade.UpgradeInfo.Name,
                Triggers.FinishTrigger
            );

            subphase.PrepareByParameters(
                SelectObstacle,
                TrySelectObstacle,
                HostShip.Owner.PlayerNo,
                true,
                HostUpgrade.UpgradeInfo.Name,
                "Select obstacle to move",
                HostUpgrade
            );

            subphase.Start();
        }

        private void SelectObstacle(GenericObstacle obstacle)
        {
            SelectObstacleSubPhase.SelectObstacleNoCallback();

            Messages.ShowInfo("An obstacle was selected");
            ChosenObstacle = obstacle;

            StartMoveObstacleSubphase();
        }

        private bool TrySelectObstacle(GenericObstacle obstacle)
        {
            return true;
        }

        private void StartMoveObstacleSubphase()
        {
            MoveObstacleMidgameSubPhase subphase = Phases.StartTemporarySubPhaseNew<MoveObstacleMidgameSubPhase>(
                "Move obstacle",
                Triggers.FinishTrigger
            );

            MoveObstacleMidgameSubPhase.ChosenObstacle = ChosenObstacle;
            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Place this obstacle beyond range 2 of any board edge of ship and beyond range 1 of other obstacles";
            subphase.ImageSource = HostUpgrade;
            subphase.SetupFilter = SetupFilter;

            subphase.Start();
        }

        private bool SetupFilter()
        {
            bool result = true;
            ShipObstacleDistance minDist = null;

            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ShipObstacleDistance dist = new ShipObstacleDistance(ship, ChosenObstacle);
                if (minDist == null || dist.DistanceReal < minDist.DistanceReal) minDist = dist;
            }

            if (minDist.Range <= 2)
            {
                result = false;
                if (minDist.DistanceReal < MoveObstacleMidgameSubPhase.DistanceFromEdge)
                MovementTemplates.ShowRangeRulerR2(minDist.NearestPointObstacle, minDist.NearestPointShip);
            }

            return result;
        }
    }
}