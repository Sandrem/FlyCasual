using Abilities;
using GameModes;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Navigator : GenericUpgrade
    {
        public Navigator() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Navigator";
            Cost = 3;

            // AvatarOffset = new Vector2(10, 1);

            UpgradeAbilities.Add(new NavigatorAbility());
        }
    }
}

namespace Abilities
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
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.Bearing == Selection.ThisShip.AssignedManeuver.Bearing && movementStruct.Direction == Selection.ThisShip.AssignedManeuver.Direction)
            {
                if (!(movementStruct.ColorComplexity == Movement.ManeuverColor.Red && HostShip.Tokens.HasToken(typeof(Tokens.StressToken))))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}