using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using SubPhases;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace KWing
    {
        public class MirandaDoni : KWing, ISecondEditionPilot
        {
            public MirandaDoni() : base()
            {
                PilotName = "Miranda Doni";
                PilotSkill = 8;
                Cost = 29;

                IsUnique = true;

                PilotAbilities.Add(new MirandaDoniAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 48;

                PilotAbilities.RemoveAll(ability => ability is Abilities.MirandaDoniAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.MirandaDoniAbilitySE());

                SEImageNumber = 62;
            }
        }
    }
}

namespace Abilities
{
    public class MirandaDoniAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.Events.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }

        private void CheckConditions()
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        protected virtual void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.InfoText = "Use " + Name + "?";

            if (HostShip.Shields > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.Shields < HostShip.MaxShields)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecisionName = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }

        protected string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.Shields < HostShip.MaxShields) result = "Roll 1 fewer die to recover 1 shield";

            return result;
        }

        protected void RegisterRollExtraDice(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            HostShip.LoseShield();

            Messages.ShowInfo("Miranda Doni spends shield to roll extra die");

            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        protected void RegisterRegeneration(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.AfterGotNumberOfAttackDice += RegenerateShield;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RegenerateShield(ref int count)
        {
            count--;
            HostShip.TryRegenShields();

            Messages.ShowInfo("Miranda Doni rolls 1 fewer die to recover 1 shield");

            HostShip.AfterGotNumberOfAttackDice -= RegenerateShield;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;
        }

        protected class MirandaDoniDecisionSubPhase : DecisionSubPhase { }
    }
}

namespace Abilities.SecondEdition
{
    public class MirandaDoniAbilitySE : MirandaDoniAbility
    {
        protected override void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            MirandaDoniDecisionSubPhase selectMirandaDoniSubPhase = (MirandaDoniDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(MirandaDoniDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectMirandaDoniSubPhase.InfoText = "Use " + Name + "?";

            if (HostShip.Shields > 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Spend 1 shield to roll 1 extra die", RegisterRollExtraDice);
                selectMirandaDoniSubPhase.AddTooltip("Spend 1 shield to roll 1 extra die", HostShip.ImageUrl);
            }

            if (HostShip.Shields == 0)
            {
                selectMirandaDoniSubPhase.AddDecision("Roll 1 fewer die to recover 1 shield", RegisterRegeneration);
                selectMirandaDoniSubPhase.AddTooltip("Roll 1 fewer die to recover 1 shield", HostShip.ImageUrl);
            }

            selectMirandaDoniSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            selectMirandaDoniSubPhase.DefaultDecisionName = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }
    }
}