using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class HanSolo : ScavengedYT1300
        {
            public HanSolo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Han Solo",
                    "Jaded Smuggler",
                    Faction.Resistance,
                    6,
                    6,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HanSoloResistancePilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "hansolo-scavengedyt1300";

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/5816bd53c272ed50096e22ae1af2b38a.png";
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
                    Messages.ShowError("The range to " + placedEnemyShip.PilotInfo.PilotName + " is " + distInfo.Range);
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