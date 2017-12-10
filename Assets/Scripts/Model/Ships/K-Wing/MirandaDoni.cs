using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using SubPhases;

namespace Ship
{
    namespace KWing
    {
        public class MirandaDoni : KWing
        {
            public MirandaDoni() : base()
            {
                PilotName = "Miranda Doni";
                PilotSkill = 8;
                Cost = 29;

                IsUnique = true;

                PilotAbilities.Add(new MirandaDoniAbility());
            }
        }
    }
}

namespace Abilities
{
    public class MirandaDoniAbility : GenericAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.OnRoundEnd += ClearAbility;
        }

        private void CheckConditions()
        {
            if (!IsAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);
            }
        }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
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

            selectMirandaDoniSubPhase.DefaultDecision = GetDefaultDecision();

            selectMirandaDoniSubPhase.ShowSkipButton = true;

            selectMirandaDoniSubPhase.Start();
        }

        private string GetDefaultDecision()
        {
            string result = "No";

            if (HostShip.Shields < HostShip.MaxShields) result = "Roll 1 fewer die to recover 1 shield";

            return result;
        }

        private void RegisterRollExtraDice(object sender, System.EventArgs e)
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

        private void RegisterRegeneration(object sender, System.EventArgs e)
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

        private class MirandaDoniDecisionSubPhase : DecisionSubPhase { }
    }
}
