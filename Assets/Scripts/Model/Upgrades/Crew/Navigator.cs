using Upgrade;
using Ship;
using GameModes;

namespace UpgradesList
{
    public class Navigator : GenericUpgrade
    {
        public Navigator() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Navigator";
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnManeuverIsRevealed += RegisterAskChangeManeuver;
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
            DirectionsMenu.Show(GameMode.CurrentGameMode.AssignManeuver, IsSameBearingAndDirection);
        }

        private bool IsSameBearingAndDirection(string maneuverString)
        {
            bool result = false;
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.Bearing == Selection.ThisShip.AssignedManeuver.Bearing && movementStruct.Direction == Selection.ThisShip.AssignedManeuver.Direction)
            {
                if (!(movementStruct.ColorComplexity == Movement.ManeuverColor.Red && Host.HasToken(typeof(Tokens.StressToken))))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
