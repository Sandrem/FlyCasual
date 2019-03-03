using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class LieutenantDormitz : UpsilonClassCommandShuttle
        {
            public LieutenantDormitz() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Dormitz",
                    2,
                    70,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantDormitzAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e4acd80da2c39e25d4f999cb7c314fe5.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantDormitzAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSetupPlaced += ActivateAbilityForAllOtherFriendlyShips;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSetupSelected -= ActivateAbilityForAllOtherFriendlyShips;

            GenericShip.OnSetupSelectedGlobal -= ApplySetupFilter;
            GenericShip.OnSetupPlacedGlobal -= RemoveSetupFilter;
        }

        private void ActivateAbilityForAllOtherFriendlyShips(GenericShip ship)
        {
            GenericShip.OnSetupSelectedGlobal += ApplySetupFilter;
            GenericShip.OnSetupPlacedGlobal += RemoveSetupFilter;
        }

        private void ApplySetupFilter(GenericShip ship)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            SetupSubPhase setupSubPhase = Phases.CurrentSubPhase as SetupSubPhase;
            setupSubPhase.SetupFilter = LieutenantDormitzRestrictions;
            setupSubPhase.SetupRangeHelper = LieutenantDormitzSetupRangeHelper;

            setupSubPhase.ShowSubphaseDescription(
                HostShip.PilotInfo.PilotName,
                "Ship can be placed anywhere in the play area at range 0-2 of " + HostShip.PilotInfo.PilotName,
                HostShip
            );
        }

        private void LieutenantDormitzSetupRangeHelper()
        {
            MovementTemplates.ReturnRangeRulerR2();

            DistanceInfo distInfo = new DistanceInfo(HostShip, Selection.ThisShip);
            if (!SetupSubPhase.IsShipInStartingZone(Selection.ThisShip))
            {
                MovementTemplates.ShowRangeRulerR2(distInfo.MinDistance.Point1, distInfo.MinDistance.Point2);
            }

        }

        private bool LieutenantDormitzRestrictions()
        {
            DistanceInfo distInfo = new DistanceInfo(Selection.ThisShip, HostShip);
            if (distInfo.Range > 2 && !SetupSubPhase.IsShipInStartingZone(Selection.ThisShip))
            {
                Messages.ShowError("Range to " + HostShip.PilotInfo.PilotName + " is " + distInfo.Range);
                return false;
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
