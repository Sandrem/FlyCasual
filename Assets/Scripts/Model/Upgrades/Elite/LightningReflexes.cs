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
            if (!HostShip.HasManeuver(HostShip.AssignedManeuver.ToString())) return;
            if (Board.BoardManager.IsOffTheBoard(host)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == ManeuverColor.White || HostShip.AssignedManeuver.ColorComplexity == ManeuverColor.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, (s, e) => AskToUseAbility(NeverUseByDefault, UseAbility));
            }
        }                

        private void UseAbility(object sender, System.EventArgs e)
        {            
            var lightningReflexesSubphase = Phases.StartTemporarySubPhaseNew("Rotate ship 180°", typeof(KoiogranTurnSubPhase), () =>
            {                
                HostShip.Tokens.AssignToken(new StressToken(HostShip), DecisionSubPhase.ConfirmDecision);
                Messages.ShowInfoToHuman(string.Format("{0} discarded Lightning Reflexes to turn ship 180° and get stress token.", HostShip.PilotName));
            });

            HostUpgrade.TryDiscard(lightningReflexesSubphase.Start);            
        }
    }
}