using Ship;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class NoviceTechnician : GenericUpgrade
    {
        public NoviceTechnician() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Novice Technician",
                UpgradeType.Crew,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.NoviceTechnicianCrewAbility),
                seImageNumber: 45
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    //At the end of the round, you may roll 1 attack die to repair 1 faceup damage card. Then on a hit result, expose 1 damage card.
    public class NoviceTechnicianCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.Damage.HasFaceupCards)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskAbility);
            }
        }

        private void AskAbility(object sender, EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<NoviceTechnicianDecisionSubPhase>(
                "Novice Technician: Select faceup damage card to repair",
                Triggers.FinishTrigger
            );
            phase.HostShip = HostShip;
            phase.DecisionOwner = HostShip.Owner;
            phase.ShowSkipButton = true;
            phase.CritDiscarded = CritDiscarded;
            phase.Start();
        }

        private void CritDiscarded(Action callback)
        {
            PerformDiceCheck(
                HostName,
                DiceKind.Attack,
                1,
                DiceCheckFinished,
                callback);
        }

        private void DiceCheckFinished()
        {
            if (DiceCheckRoll.Successes > 0)
            {
                Messages.ShowInfo(HostName + " exposes one damage card.");
                HostShip.Damage.ExposeRandomFacedownCard(AbilityDiceCheck.ConfirmCheck);
            }
            else
            {
                AbilityDiceCheck.ConfirmCheck();
            }
        }

        private class NoviceTechnicianDecisionSubPhase : DecisionSubPhase
        {
            public GenericShip HostShip { get; set; }
            public Action<Action> CritDiscarded { get; set; }

            public override void PrepareDecision(Action callBack)
            {
                InfoText = "Novice Technician: Select faceup damage card to repair.";

                DecisionViewType = DecisionViewTypes.ImagesDamageCard;

                foreach (var crit in HostShip.Damage.GetFaceupCrits().ToList())
                {
                    AddDecision(crit.Name, delegate { DiscardCrit(crit); }, crit.ImageUrl);
                }

                DefaultDecisionName = GetDecisions().First().Name;

                callBack();
            }

            private void DiscardCrit(GenericDamageCard critCard)
            {
                Selection.ActiveShip.Damage.FlipFaceupCritFacedown(critCard);
                CritDiscarded(ConfirmDecision);
            }

        }
    }
}
