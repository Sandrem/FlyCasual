using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class MillenniumFalconHotR : GenericUpgrade
    {
        public MillenniumFalconHotR() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Millennium Falcon (HotR)",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YT1300.YT1300)),
                abilityType: typeof(Abilities.FirstEdition.MillenniumFalconHotRAbility)
            );

            // TODOREVERT
            // NameCanonical = "millenniumfalcon-swx57";
        }        
    }
}

namespace Abilities.FirstEdition
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
            Selection.ThisShip.Tokens.AssignToken(typeof(StressToken), StartRotate180SubPhase);
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
