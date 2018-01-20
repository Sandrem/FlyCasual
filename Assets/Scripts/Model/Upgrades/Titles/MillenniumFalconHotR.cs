using Ship;
using Ship.YT1300;
using Upgrade;
using System;

namespace UpgradesList
{ 
    public class MillenniumFalconHotR : GenericUpgrade
    {
        public MillenniumFalconHotR() : base()
        {
            Type = UpgradeType.Title;
            Name = "Millennium Falcon (HotR)";
            Cost = 1;
            isUnique = true;

            NameCanonical = "millenniumfalcon-swx57";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT1300;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnMovementFinish += CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.Speed == 3 && ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Bank)
            {
                if (!ship.IsBumped)
                {
                    Triggers.RegisterTrigger(new Trigger() {
                        Name = "Millenium Falcon's ability",
                        TriggerType = TriggerTypes.OnShipMovementFinish,
                        TriggerOwner = Host.Owner.PlayerNo,
                        Sender = ship,
                        EventHandler = RotateShip180
                    });
                }
            }
        }

        private void RotateShip180(object sender, System.EventArgs e)
        {
            GenericShip thisShip = sender as GenericShip;

            if (!thisShip.HasToken(typeof(Tokens.StressToken)))
            {
                Phases.StartTemporarySubPhaseOld("Rotate ship 180° decision", typeof(SubPhases.MillenniumFalconHotRDecisionSubPhase), Triggers.FinishTrigger);
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot use ability: pilot is stressed");
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace SubPhases
{

    public class MillenniumFalconHotRDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Receive stress token to rotate ship 180°?";

            AddDecision("Yes", RotateShip180);
            AddDecision("No", DontRotateShip180);

            DefaultDecision = "No";

            callBack();
        }

        private void RotateShip180(object sender, EventArgs e)
        {
            Selection.ThisShip.AssignToken(new Tokens.StressToken(), StartRotate180SubPhase);
        }

        private void StartRotate180SubPhase()
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 180°", typeof(KoiogranTurnSubPhase), ConfirmDecision);
        }

        private void DontRotateShip180(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
