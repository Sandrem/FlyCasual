using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using System;
using SubPhases;
using DamageDeckCard;
using System.Linq;

namespace Ship
{
    namespace UpsilonShuttle
    {
        public class KyloRenUpsilon : UpsilonShuttle
        {
            public KyloRenUpsilon() : base()
            {
                PilotName = "Kylo Ren";
                PilotSkill = 6;
                Cost = 34;

                IsUnique = true;

                UpgradeBar.AddSlot(UpgradeType.Elite);

                PilotAbilities.Add(new KyloRenPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KyloRenPilotAbility: GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnRoundEnd += RegisterShowPilotCrits;
        }

        public override void DeactivateAbility()
        {
            
        }

        private void RegisterShowPilotCrits()
        {
            RegisterAbilityTrigger(TriggerTypes.OnRoundEnd, ShowPilotCrits);
        }

        private void ShowPilotCrits(object sender, EventArgs e)
        {
            SelectPilotCritDecision selectPilotCritSubphase = (SelectPilotCritDecision) Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectPilotCritDecision),
                Triggers.FinishTrigger
            );

            List<GenericDamageCard> opponentDeck = DamageDecks.GetDamageDeck(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Deck;
            foreach (var card in opponentDeck.Where(n => n.Type == CriticalCardType.Pilot))
            {
                Decision existingDecision = selectPilotCritSubphase.GetDecisions().Find(n => n.Name == card.Name);
                if (existingDecision == null)
                {
                    selectPilotCritSubphase.AddDecision(card.Name, delegate { DecisionSubPhase.ConfirmDecision(); }, card.ImageUrl, 1);
                }
                else
                {
                    existingDecision.SetCount(existingDecision.Count + 1);
                }
            }

            selectPilotCritSubphase.DecisionViewType = DecisionViewTypes.ImageButtons;

            selectPilotCritSubphase.DefaultDecisionName = selectPilotCritSubphase.GetDecisions().First().Name;

            selectPilotCritSubphase.InfoText = "Kylo Ren: Select Damage Card";

            selectPilotCritSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectPilotCritSubphase.Start();
        }

        private class SelectPilotCritDecision : DecisionSubPhase { };
    }
}
