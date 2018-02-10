using Abilities;
using DamageDeckCard;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Conditions;

namespace UpgradesList
{
    public class KyloRenCrew : GenericUpgrade
    {
        public KyloRenCrew() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Kylo Ren";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new KyloRenCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class KyloRenCrewAbility : GenericAbility
    {
        public GenericDamageCard AssignedDamageCard;

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += KyloRenCrewAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= KyloRenCrewAddAction;
        }

        private void KyloRenCrewAddAction(GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.KyloRenCrewAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                DoAction = DoKyloRenAction
            };
            host.AddAvailableAction(action);
        }

        private void DoKyloRenAction()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, SelectShip);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Phases.CurrentSubPhase.CallBack);
        }

        private void SelectShip(object sender, EventArgs e)
        {
            // TODO: Skip/Wrong target - revert

            SelectTargetForAbilityNew(
                AssignConditionToTarget,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo
            );
        }

        private void AssignConditionToTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            ShowPilotCrits();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 50;
        }

        private void ShowPilotCrits()
        {
            SelectPilotCritDecision selectPilotCritSubphase = (SelectPilotCritDecision)Phases.StartTemporarySubPhaseNew(
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
                    selectPilotCritSubphase.AddDecision(card.Name, delegate { SelectDamageCard(card); }, card.ImageUrl, 1);
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

        private void SelectDamageCard(GenericDamageCard damageCard)
        {
            Messages.ShowInfo("Card selected: " + damageCard.Name);

            // TODO: Get and Remove damage card from deck

            AssignedDamageCard = damageCard;

            TargetShip.Tokens.AssignCondition(new IllShowYouTheDarkSide(TargetShip));
            TargetShip.Tokens.AssignCondition(new IllShowYouTheDarkSideDamageCard(TargetShip) { Tooltip = damageCard.ImageUrl });

            DecisionSubPhase.ConfirmDecision();
        }

        private class SelectPilotCritDecision : DecisionSubPhase { };
    }
}

namespace ActionsList
{
    public class KyloRenCrewAction : GenericAction
    {
        public KyloRenCrewAction()
        {
            Name = EffectName = "Kylo Ren: Assign condition";
        }

        public override int GetActionPriority()
        {
            return 0;
        }
    }
}

namespace Conditions
{
    public class IllShowYouTheDarkSide : Tokens.GenericToken
    {
        public IllShowYouTheDarkSide(GenericShip host) : base(host)
        {
            Name = "Harpooned Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/ill-show-you-the-dark-side.png";
        }
    }

    public class IllShowYouTheDarkSideDamageCard : Tokens.GenericToken
    {
        public IllShowYouTheDarkSideDamageCard(GenericShip host) : base(host)
        {
            Name = "Harpooned Condition";
            Temporary = false;
        }
    }
}