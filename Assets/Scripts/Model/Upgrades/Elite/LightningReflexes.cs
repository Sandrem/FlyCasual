using Abilities;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class LightningReflexes : GenericUpgrade
    {
        public LightningReflexes() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Lightning Reflexes";
            Cost = 1;

            UpgradeAbilities.Add(new LightningReflexesAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
{
    // After you execute a white or green maneuver on your dial, you may discard this card to rotate your ship 180 degrees. Then receive 1 stress token after the "Check Pilot Stress" step.
    public class LightningReflexesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckTrigger;
        }

        private void CheckTrigger(GenericShip host)
        {
            /* FAQ:
             * A ship that executes a maneuver that is not on its dial (such as an ionized ship, a ship using Inertial Dampeners, 
             * or Juno Eclipse using her pilot ability to execute a maneuver that is not on her dial) cannot use Lightning Reflexes. 
             * (X-Wing FAQ, Version 3.2, Updated 09/04/2015) */
            if(!HostShip.HasManeuver(HostShip.AssignedManeuver.ToString()))
            {
                return;
            }

            if (HostShip.AssignedManeuver.ColorComplexity != ManeuverColor.Red)
            {                
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Lightning Reflexes' ability",
                    TriggerType = TriggerTypes.OnShipMovementFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    Sender = HostShip,
                    EventHandler = RotateShip180
                });                
            }
        }

        private void RotateShip180(object sender, System.EventArgs e)
        {
            GenericShip thisShip = sender as GenericShip;
            Phases.StartTemporarySubPhaseOld("Rotate ship 180° decision", typeof(SubPhases.LightningReflexesDecisionSubPhase), Triggers.FinishTrigger);
        }
    }
}

namespace SubPhases
{
    public class LightningReflexesDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Discard Lightning Reflexes to rotate ship 180° and receive stress token?";

            AddDecision("Yes", RotateShip180);
            AddDecision("No", DontRotateShip180);

            DefaultDecisionName = "No";

            callBack();
        }

        private void RotateShip180(object sender, EventArgs e)
        {
            var lightningReflexesUpgrade = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyFaceup()
                .FirstOrDefault(u => u.Types.Contains(UpgradeType.Elite) && u.Name == "Lightning Reflexes");
            if (lightningReflexesUpgrade != null)
            {
                lightningReflexesUpgrade.TryDiscard(StartRotate180SubPhase);
            }
        }

        private void StartRotate180SubPhase()
        {
            Messages.ShowInfoToHuman(string.Format("{0} discarded Lightning Reflexes to turn ship 180° and get stress token.", Selection.ThisShip.PilotName));
            Phases.StartTemporarySubPhaseOld("Rotate ship 180°", typeof(KoiogranTurnSubPhase), 
                () => Selection.ThisShip.Tokens.AssignToken(new Tokens.StressToken(Selection.ThisShip), ConfirmDecision)
            );
        }

        private void DontRotateShip180(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

    }

}