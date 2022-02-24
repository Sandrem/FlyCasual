using Upgrade;
using System.Linq;
using System.Collections.Generic;
using System;
using Tokens;
using Ship;
using GameModes;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class R2A6 : GenericUpgrade
    {
        public R2A6() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-A6",
                UpgradeType.Astromech,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.R2A6Ability)
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c4/Swz40_card-r2-a6.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2A6Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostShip.PilotInfo.PilotName + " : You can change your maneuver");

            HostShip.Owner.ChangeManeuver(
                (maneuverCode) => {
                    ShipMovementScript.SendAssignManeuverCommand(maneuverCode);
                },
                Triggers.FinishTrigger,
                IsSpeedPlusMinus1
            );
        }

        private bool IsSpeedPlusMinus1(string maneuverString)
        {
            GenericMovement maneuver = ShipMovementScript.MovementFromString(maneuverString);
            if (maneuver.Bearing == HostShip.AssignedManeuver.Bearing
                && maneuver.Direction == HostShip.AssignedManeuver.Direction)
            {
                if (maneuver.Speed == HostShip.AssignedManeuver.Speed
                    || maneuver.Speed == HostShip.AssignedManeuver.Speed - 1
                    || maneuver.Speed == HostShip.AssignedManeuver.Speed + 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}