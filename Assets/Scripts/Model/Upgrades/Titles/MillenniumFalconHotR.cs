using Ship;
using Ship.YT1300;
using Upgrade;
using System;
using Abilities;

namespace UpgradesList
{
    public class MillenniumFalconHotR : GenericUpgrade
    {
        public MillenniumFalconHotR() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Millennium Falcon (HotR)";
            Cost = 1;
            isUnique = true;

            NameCanonical = "millenniumfalcon-swx57";

            UpgradeAbilities.Add(new MillenniumFalconHotRAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT1300;
        }
    }
}

namespace Abilities
{
    public class MillenniumFalconHotRAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.Speed != 3) return;
            if (ship.AssignedManeuver.Bearing != Movement.ManeuverBearing.Bank) return;
            if (ship.IsBumped) return;
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Millenium Falcon's ability",
                    TriggerType = TriggerTypes.OnMovementFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    Sender = ship,
                    EventHandler = RotateShip180
                }
            );
        }

        private void RotateShip180(object sender, EventArgs e)
        {
            GenericShip thisShip = sender as GenericShip;

            if (!thisShip.Tokens.HasToken(typeof(Tokens.StressToken)))
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

            DefaultDecisionName = "No";

            callBack();
        }

        private void RotateShip180(object sender, EventArgs e)
        {
            Selection.ThisShip.Tokens.AssignToken(new Tokens.StressToken(Selection.ThisShip), StartRotate180SubPhase);
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
