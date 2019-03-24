using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class EmperorPalpatine : GenericUpgrade
    {
        public EmperorPalpatine() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Emperor Palpatine",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.FirstEdition.EmperorPalpatineCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(68, 9));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class EmperorPalpatineCrewAbility : GenericAbility
    {

        public GenericShip ShipUsingPalpatine;
        public DieSide PalpatineDieChoice;
        public DiceKind DiceType;

        public override void ActivateAbility()
        {
            GenericShip.OnDiceAboutToBeRolled += CheckEmperorPalpatineAbility;
            HostShip.OnShipIsDestroyed += RemoveEmperorPalpatineAbilityWhenDestroyed;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            RemoveEmperorPalpatineAbility();
        }

        private void RemoveEmperorPalpatineAbilityWhenDestroyed(GenericShip ship, bool isFled)
        {
            RemoveEmperorPalpatineAbility();
        }

        private void RemoveEmperorPalpatineAbility()
        {
            GenericShip.OnDiceAboutToBeRolled -= CheckEmperorPalpatineAbility;
            HostShip.OnShipIsDestroyed -= RemoveEmperorPalpatineAbilityWhenDestroyed;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckEmperorPalpatineAbility()
        {
            if (!IsAbilityUsed)
            {
                if ((HostShip.Owner.Id == Combat.Attacker.Owner.Id && Combat.AttackStep == CombatStep.Attack) || // We're attacking
                   ((HostShip.Owner.Id == Combat.Defender.Owner.Id && Combat.AttackStep == CombatStep.Defence))) // or we're defending
                {
                    RegisterAbilityTrigger(TriggerTypes.OnDiceAboutToBeRolled, StartQuestionSubphase);
                }
            }
        }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            if (HostShip.Owner.Id == Combat.Attacker.Owner.Id && Combat.AttackStep == CombatStep.Attack) // We're attacking
            {
                DiceType = DiceKind.Attack;
                ShipUsingPalpatine = Combat.Attacker;
            }
            else
            {
                if (HostShip.Owner.Id == Combat.Defender.Owner.Id && Combat.AttackStep == CombatStep.Defence) // We're defending
                {
                    DiceType = DiceKind.Defence;
                    ShipUsingPalpatine = Combat.Defender;
                }
                else
                {
                    // We're doing something else - for future non-combat Palpatine
                    Triggers.FinishTrigger();
                    return;
                }
            }


            EmperorPalpatineDecisionSubPhase newSubPhase = (EmperorPalpatineDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(EmperorPalpatineDecisionSubPhase),
                Triggers.FinishTrigger
            );

            newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            newSubPhase.InfoText = "Use " + Name + "?";
            newSubPhase.ShowSkipButton = true;
            newSubPhase.OnSkipButtonIsPressed = DontUseEmperorPalpatine;

            if (DiceType == DiceKind.Attack)
            {
                newSubPhase.AddDecision("Critical Hit", ChoiceCriticalHit);
                newSubPhase.AddDecision("Hit", ChoiceHit);
            }
            else
            {
                if (DiceType == DiceKind.Defence)
                {
                    newSubPhase.AddDecision("Evade", ChoiceEvade);
                }
            }

            newSubPhase.AddDecision("Focus", ChoiceFocus);
            newSubPhase.AddDecision("Blank", ChoiceBlank);

            newSubPhase.DefaultDecisionName = GetDefaultDecision();
            newSubPhase.Start();
        }

        private void DieChoiceHelper(DieSide dieSide, string dieDescription)
        {
            IsAbilityUsed = true;
            PalpatineDieChoice = dieSide;
            Messages.ShowInfo(string.Format("Emperor Palpatine chooses '{0}'.", dieDescription));
            ShipUsingPalpatine.OnImmediatelyAfterRolling += HandlePalpatineDiceChange;
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChoiceCriticalHit(object sender, System.EventArgs e)
        {
            DieChoiceHelper(DieSide.Crit, "Critical Hit");
        }

        private void ChoiceHit(object sender, System.EventArgs e)
        {
            DieChoiceHelper(DieSide.Success, "Hit");
        }

        private void ChoiceEvade(object sender, System.EventArgs e)
        {
            DieChoiceHelper(DieSide.Success, "Evade");
        }

        private void ChoiceFocus(object sender, System.EventArgs e)
        {
            DieChoiceHelper(DieSide.Focus, "Focus");
        }

        private void ChoiceBlank(object sender, System.EventArgs e)
        {
            DieChoiceHelper(DieSide.Blank, "Blank");
        }

        private void DontUseEmperorPalpatine()
        {
            PalpatineDieChoice = DieSide.Unknown;
            Messages.ShowInfo("Emperor Palpatine was not used.");
        }

        private string GetDefaultDecision()
        {
            // TODO: Add handling for decisions for non-combat (asteriod, etc)
            string result = "No";

            int ourPilotSkill = 0;
            int theirPilotSkill = 0;
            int randomDecisionValue = UnityEngine.Random.Range(1, 100);

            if (DiceType == DiceKind.Attack)
            {
                // Use defender's agility value and the likelyhood that there are better reasons to use Palp later to decide.
                ourPilotSkill = Combat.Attacker.State.Initiative;
                theirPilotSkill = Combat.Defender.State.Initiative;
                float avgPilotSkillUs = Combat.Attacker.Owner.AveragePilotSkillOfRemainingShips();
                float avgPilotSkillThem = Combat.Attacker.Owner.AveragePilotSkillOfRemainingEnemyShips();
                if (ourPilotSkill >= avgPilotSkillUs) // We're above our ships average pilot skill. We're likely one of the first to attack. Should lean toward saving Palp.
                {
                    if (ourPilotSkill >= avgPilotSkillThem) // We're above their average pilot skill.. defenses are more likely. Should lean more toward saving Palp.
                    {
                        if (randomDecisionValue <= (Combat.Defender.State.Agility * 3)) result = "Critical Hit";

                    }
                    else // We're below their average pilot skill.. defenses are less likely. Can take more of a chance to use Palp.
                    {
                        if (randomDecisionValue <= (Combat.Defender.State.Agility * 8)) result = "Critical Hit";
                    }

                }
                else // We're below our ships average pilot skill. We're one of the last to attack. Can lean a bit more towards using Palp.
                {
                    if (ourPilotSkill >= avgPilotSkillThem) // We're above their average pilot skill.. defenses are more likely. Should lean more toward saving Palp.
                    {
                        if (randomDecisionValue <= (Combat.Defender.State.Agility * 8)) result = "Critical Hit";
                    }
                    else // We're below their average pilot skill.. defenses are less likely. Can lean even more towards using Palp.
                    {
                        if (randomDecisionValue <= (Combat.Defender.State.Agility * 22)) result = "Critical Hit";
                    }
                }
                if (result != "Critical Hit")
                {
                    // Let's add a bit of risk-taking to the AI.
                    if (Combat.Defender.State.ShieldsCurrent == 0 && Combat.Defender.State.HullCurrent <= 2) // If the defender is on it's last legs, use Palp most of the time.
                    {
                        if (Combat.Defender.State.Initiative < ourPilotSkill) // Can we prevent a shot? Go for it..
                        {
                            if (randomDecisionValue <= (Combat.Defender.State.Agility * 30)) result = "Critical Hit";
                        }
                        else
                        {
                            if (randomDecisionValue <= (Combat.Defender.State.Agility * 22)) result = "Critical Hit";
                        }
                    }
                }
            }
            else
            {
                // Use attacker's roll, our agility, and our ships condition to decide
                int hitsRolled = Combat.DiceRollAttack.RegularSuccesses;
                int critsRolled = Combat.DiceRollAttack.CriticalSuccesses;
                int totalHits = hitsRolled + critsRolled;
                int totalHitsMinusPotentialEvades = totalHits - Combat.Defender.State.Agility;
                int ourHitsLeft = Combat.Defender.State.HullCurrent + Combat.Defender.State.ShieldsCurrent;

                if (Combat.Defender.State.ShieldsCurrent > 0) // We have some shields left. Can afford to take a crit if needed.
                {
                    if (critsRolled > Combat.Defender.State.ShieldsCurrent) // If we don't evade, we will take a crit.
                    {
                        if (randomDecisionValue <= (totalHitsMinusPotentialEvades * 50)) result = "Evade";
                    }
                    else // We won't take a crit even without an evade
                    {
                        if (randomDecisionValue <= (totalHitsMinusPotentialEvades * 10)) result = "Evade";
                    }
                }
                else // We don't have any shields left (can take crits), should be more aggressive with Palp.
                {
                    if (randomDecisionValue <= (totalHitsMinusPotentialEvades * 70)) result = "Evade";
                    if (totalHitsMinusPotentialEvades > ourHitsLeft) result = "No"; // Don't waste Palp - result is hopeless anyhow.
                }
            }
            return result;
        }

        private void HandlePalpatineDiceChange(DiceRoll diceroll)
        {
            ShipUsingPalpatine.OnImmediatelyAfterRolling -= HandlePalpatineDiceChange;
            if (PalpatineDieChoice != DieSide.Unknown)
            {
                DieSide dieToChange = diceroll.FindDieToChange(PalpatineDieChoice);
                if (dieToChange == DieSide.Unknown)
                {
                    Messages.ShowErrorToHuman("Error selecting die to change for Emperor Palpatine.");
                    return;
                }
                Messages.ShowInfo(string.Format("Emperor Palpatine changes one '{0}' to {1}.", dieToChange, PalpatineDieChoice));
                diceroll.ChangeOne(dieToChange, PalpatineDieChoice, true, true);
                PalpatineDieChoice = DieSide.Unknown;
                return;
            }
            Messages.ShowErrorToHuman("Error handling die change for Emperor Palpatine");
            return;
        }

        private class EmperorPalpatineDecisionSubPhase : DecisionSubPhase { }
    }
}