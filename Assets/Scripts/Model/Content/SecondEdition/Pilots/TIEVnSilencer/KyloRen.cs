using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class KyloRen : TIEVnSilencer
        {
            public KyloRen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kylo Ren",
                    5,
                    82,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KyloRenPilotAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.Force //,
                                                        //seImageNumber: 120
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/71dbde337b9ff5aab897781d40d8f653.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenPilotAbility : Abilities.FirstEdition.KyloRenPilotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckKyloRenAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckKyloRenAbility;
        }

        protected void CheckKyloRenAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskKyloRenAbility);
            }
        }

        protected void AskKyloRenAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AssignConditionToAttacker, null, null, false);
        }

        protected override void AssignConditionToAttacker(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Ill-Show-You-The-Dark-Side");
            HostShip.State.Force--;

            if (AssignedDamageCard == null)
            {
                // If condition is not in play - select card to assign
                ShowPilotCrits();
            }
            else
            {
                // If condition is in play - reassing only
                RemoveConditions(ShipWithCondition);
                AssignConditions(Combat.Attacker);
                DecisionSubPhase.ConfirmDecision();
            }
        }

        protected override void ShowPilotCrits()
        {
            SelectPilotCritDecision selectPilotCritSubphase = (SelectPilotCritDecision)Phases.StartTemporarySubPhaseNew(
                "Select Damage Card",
                typeof(SelectPilotCritDecision),
                DecisionSubPhase.ConfirmDecision
            );

            List<GenericDamageCard> opponentDeck = DamageDecks.GetDamageDeck(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Deck;
            foreach (var card in opponentDeck.Where(n => n.Type == CriticalCardType.Pilot))
            {
                Decision existingDecision = selectPilotCritSubphase.GetDecisions().Find(n => n.Name == card.Name);
                if (existingDecision == null)
                {
                    selectPilotCritSubphase.AddDecision(card.Name, delegate { SelectDamageCard(card); }, card.ImageUrl, 1);
                }
                else
                {
                    existingDecision.SetCount(existingDecision.Count + 1);
                }
            }

            selectPilotCritSubphase.DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            selectPilotCritSubphase.DefaultDecisionName = selectPilotCritSubphase.GetDecisions().First().Name;

            selectPilotCritSubphase.InfoText = "Kylo Ren: Select Damage Card";

            selectPilotCritSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectPilotCritSubphase.Start();
        }
    }
}