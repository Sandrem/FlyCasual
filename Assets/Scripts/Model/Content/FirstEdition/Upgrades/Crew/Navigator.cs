using Ship;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;
using GameModes;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class Navigator : GenericUpgrade
    {
        public Navigator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Navigator",
                UpgradeType.Crew,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.NavigatorAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(10, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class NavigatorAbility : GenericAbility
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
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Navigator's ability",
                TriggerType = TriggerTypes.OnManeuverIsRevealed,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = AskChangeManeuver
            });
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            HostShip.Owner.ChangeManeuver(GameMode.CurrentGameMode.AssignManeuver, IsSameBearingAndDirection);
        }

        private bool IsSameBearingAndDirection(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Bearing == Selection.ThisShip.AssignedManeuver.Bearing && movementStruct.Direction == Selection.ThisShip.AssignedManeuver.Direction)
            {
                if (!(movementStruct.ColorComplexity == Movement.MovementComplexity.Complex && HostShip.Tokens.HasToken(typeof(Tokens.StressToken))))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}