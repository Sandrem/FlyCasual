using Ship;
using SubPhases;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CountDooku : GenericUpgrade
    {
        public CountDooku() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Count Dooku",
                UpgradeType.Crew,
                cost: 14,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.CountDookuCrewAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(307, 1),
                new Vector2(150, 150)
            );
        }
    }
}


namespace Abilities.SecondEdition
{
    //Before a ship at range 0-2 rolls attack or defense dice, if all of your [Force] are active, you may spend 1 [Force] and name a result. 
    //If the roll does not contain the named result, the ship must change 1 die to that result.
    public class CountDookuCrewAbility : GenericAbility
    {
        private DieSide NamedResult;

        public override void ActivateAbility()
        {
            GenericShip.OnDiceAboutToBeRolled += CheckDookuAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDiceAboutToBeRolled -= CheckDookuAbility;
        }
        
        private void CheckDookuAbility()
        {
            GenericShip targetShip = null;
            if (Combat.AttackStep == CombatStep.Attack) targetShip = Combat.Attacker;
            else if (Combat.AttackStep == CombatStep.Defence) targetShip = Combat.Defender;

            if (targetShip != null && HostShip.GetRangeToShip(targetShip) <= 2 && HostShip.State.Force == HostShip.State.MaxForce)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDiceAboutToBeRolled, StartQuestionSubphase);
                TargetShip = targetShip;
            }
        }

        private void StartQuestionSubphase(object sender, EventArgs e)
        {
            if (HostShip.State.Force == HostShip.State.MaxForce)
            {
                var newSubPhase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(DecisionSubPhase),
                    Triggers.FinishTrigger
                );

                newSubPhase.DescriptionShort = HostName;
                newSubPhase.DescriptionLong = "You may spend 1 force to name a result. If the roll does not contain the named result, the ship must change 1 die to that result";
                newSubPhase.ImageSource = HostShip;

                newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
                newSubPhase.ShowSkipButton = true;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    newSubPhase.AddDecision("Critical Hit", delegate { NameResult(DieSide.Crit, "Critical Hit"); });
                    newSubPhase.AddDecision("Hit", delegate { NameResult(DieSide.Success, "Hit"); });
                }
                else if (Combat.AttackStep == CombatStep.Defence)
                {
                    newSubPhase.AddDecision("Evade", delegate { NameResult(DieSide.Success, "Evade"); });
                }

                newSubPhase.AddDecision("Focus", delegate { NameResult(DieSide.Focus, "Focus"); });
                newSubPhase.AddDecision("Blank", delegate { NameResult(DieSide.Blank, "Blank"); });

                newSubPhase.DefaultDecisionName = GetDefaultDecision();
                newSubPhase.OnSkipButtonIsPressed = Skip;
                newSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void Skip()
        {
            NamedResult = DieSide.Unknown;
        }

        private string GetDefaultDecision()
        {
            //Very simple AI decision code.
            
            if (TargetShip.Owner == HostShip.Owner)
            {
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    return "Critical Hit";
                }
                else
                {
                    return "Evade";
                }
            }
            else
            {
                return "Blank";
            }
        }

        private void NameResult(DieSide dieSide, string dieDescription)
        {
            NamedResult = dieSide;
            Messages.ShowInfo($"{HostName} chooses '{dieDescription}'.");
            TargetShip.OnImmediatelyAfterRolling += HandleDiceChange;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }


        private void HandleDiceChange(DiceRoll diceroll)
        {
            TargetShip.OnImmediatelyAfterRolling -= HandleDiceChange;
            if (NamedResult != DieSide.Unknown && !diceroll.HasResult(NamedResult))
            {
                diceroll.ChangeWorstResultTo(NamedResult);
                
                Messages.ShowInfo($"{HostName} changes one result to {NamedResult}");
            }
            NamedResult = DieSide.Unknown;
        }
    }
}
