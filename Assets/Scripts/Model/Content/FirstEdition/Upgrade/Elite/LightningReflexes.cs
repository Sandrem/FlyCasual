using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using Movement;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class LightningReflexes : GenericUpgrade
    {
        public LightningReflexes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lightning Reflexes",
                UpgradeType.Elite,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.LightningReflexesAbility),
                restrictionSize: BaseSize.Small
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (BoardTools.Board.IsOffTheBoard(host)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Normal || HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, (s, e) => AskToUseAbility(NeverUseByDefault, UseAbility));
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            var lightningReflexesSubphase = Phases.StartTemporarySubPhaseNew("Rotate ship 180°", typeof(KoiogranTurnSubPhase), () =>
            {
                HostShip.Tokens.AssignToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision);
                Messages.ShowInfoToHuman(string.Format("{0} discarded Lightning Reflexes to turn ship 180° and get stress token.", HostShip.PilotName));
            });

            HostUpgrade.TryDiscard(lightningReflexesSubphase.Start);
        }
    }
}