using Bombs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class EdonKappehl : Mg100StarFortress
        {
            public EdonKappehl() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Edon Kappehl",
                    3,
                    69,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EdonKappehlAbility),
                    pilotTitle: "Crimson Hailstorm"
                    );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/f58fe0b57dc4a9c878627f0fea9cf1ef.png";
            }
        }
    }

}

namespace Abilities.SecondEdition
{
    public class EdonKappehlAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckEdonKappehlAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckEdonKappehlAbility;
        }

        private void CheckEdonKappehlAbility(GenericShip ship)
        {
            if(HostShip.IsBombAlreadyDropped)
            {
                return;
            }
            if(!BombsManager.HasBombsToDrop(ship))
            {
                return;
            }
            if(ship.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Easy &&
                ship.AssignedManeuver.ColorComplexity != Movement.MovementComplexity.Normal)
            {
                return;
            }

            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskEdonKappehlAbility);
        }

        private void AskEdonKappehlAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseEdonKappehlAbility);
        }

        private void UseEdonKappehlAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            var selectBombToDrop = (EdonKappehlBombDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select device to drop",
                typeof(EdonKappehlBombDecisionSubPhase),
                StartDropBombSubphase
            );

            foreach (var device in BombsManager.GetBombsToDrop(HostShip))
            {
                selectBombToDrop.AddDecision(
                    device.UpgradeInfo.Name,
                    delegate { SelectDevice(device); }
                );
            }

            selectBombToDrop.InfoText = "Select device to drop";

            selectBombToDrop.DefaultDecisionName = BombsManager.GetBombsToDrop(HostShip).First().UpgradeInfo.Name;

            selectBombToDrop.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        protected virtual void StartDropBombSubphase()
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                SpendBombCharge
            );
        }

        private void SelectDevice(GenericUpgrade deviceUpgrade)
        {
            BombsManager.CurrentBomb = deviceUpgrade as GenericTimedBomb;
            DecisionSubPhase.ConfirmDecision();
        }

        private void SpendBombCharge()
        {
            if(BombsManager.CurrentBomb != null)
            {
                BombsManager.CurrentBomb.State.SpendCharge();
            }
            Triggers.FinishTrigger();
        }

        protected class EdonKappehlBombDecisionSubPhase : DecisionSubPhase { }
    }
}
