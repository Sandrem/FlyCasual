using BoardTools;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class HanSolo : ScavengedYT1300
        {
            public HanSolo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Han Solo",
                    6,
                    76,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloResistancePilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/5816bd53c272ed50096e22ae1af2b38a.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloResistancePilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSetupSelected += ApplySetupFilter;
            HostShip.OnSetupPlaced += RemoveSetupFilter;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSetupSelected -= ApplySetupFilter;
            HostShip.OnSetupPlaced -= RemoveSetupFilter;
        }

        private void ApplySetupFilter(GenericShip ship)
        {
            SetupSubPhase setupSubPhase = Phases.CurrentSubPhase as SetupSubPhase;
            setupSubPhase.SetupFilter = HanSoloSetupRestrictions;
            setupSubPhase.SetupRangeHelper = HanSoloSetupRangeHelper;

            setupSubPhase.ShowSubphaseDescription(
                HostShip.PilotInfo.PilotName,
                "You can be placed anywhere in the play area beyond range 3 of enemy ships",
                HostShip
            );
        }

        private void HanSoloSetupRangeHelper()
        {
            List<DistanceInfo> distancesToShips = new List<DistanceInfo>();

            foreach (GenericShip placedEnemyShip in HostShip.Owner.AnotherPlayer.Ships.Values.Where(n => n.IsSetupPerformed))
            {
                distancesToShips.Add(new DistanceInfo(HostShip, placedEnemyShip));
            }

            MovementTemplates.ReturnRangeRuler();

            if (distancesToShips.Count > 0)
            {
                DistanceInfo minDistance = distancesToShips.OrderBy(n => n.MinDistance.DistanceReal).First();
                if (minDistance.Range <= 3)
                {
                    MovementTemplates.ShowRangeRuler(minDistance.MinDistance);
                }
            }
        }

        private bool HanSoloSetupRestrictions()
        {
            foreach (GenericShip placedEnemyShip in HostShip.Owner.AnotherPlayer.Ships.Values.Where(n => n.IsSetupPerformed))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, placedEnemyShip);
                if (distInfo.Range <= 3)
                {
                    Messages.ShowError("Range to " + placedEnemyShip.PilotInfo.PilotName + " is " + distInfo.Range);
                    return false;
                }
            }

            return true;
        }

        private void RemoveSetupFilter(GenericShip ship)
        {
            SetupSubPhase setupSubPhase = Phases.CurrentSubPhase as SetupSubPhase;
            setupSubPhase.SetupFilter = null;
            setupSubPhase.SetupRangeHelper = null;
            GenericSubPhase.HideSubphaseDescription();
        }
    }
}