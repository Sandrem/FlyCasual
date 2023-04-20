using System;
using System.Collections.Generic;
using Ship;
using Upgrade;
using BoardTools;
using SubPhases;
using Tokens;
using System.Linq;
using Content;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class CountDookuSoC : SithInfiltrator
    {
        public CountDookuSoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Count Dooku",
                "Siege of Coruscant",
                Faction.Separatists,
                5,
                6,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CountDookuSoCPilotAbility),
                force: 3,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Title
                },
                tags: new List<Tags>
                {
                    Tags.DarkSide,
                    Tags.Sith
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(Malice));
            MustHaveUpgrades.Add(typeof(RoilingAnger));
            MustHaveUpgrades.Add(typeof(Scimitar));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/9/93/Countdooku-siegeofcoruscant.png";

            PilotNameCanonical = "countdooku-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CountDookuSoCPilotAbility : GenericAbility
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

            if (Combat.AttackStep == CombatStep.None) return;
            else if (Combat.AttackStep == CombatStep.Attack) targetShip = Combat.Attacker;
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
