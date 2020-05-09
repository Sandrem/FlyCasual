using ActionsList;
using GameCommands;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class DiceModificationsManager
{
    public List<IDiceRollStep> DiceModificationSteps { get; private set; }
    public IDiceRollStep CurrentDiceModificationStep { get; private set; }

    public Dictionary<string, GenericAction> AvailableDiceModifications { get; set; }

    public DiceModificationsManager()
    {
        DiceModificationSteps = new List<IDiceRollStep>()
        {
            new StartDiceRollStep(typeof(AttackDiceRollCombatSubPhase)),
            new DiceModificationStep(CombatStep.Attack, PlayerRole.Attacker, DiceModificationTimingType.AfterRolled, typeof(DiceModificationAttackByDefenderSubphase)),
            new DiceModificationStep(CombatStep.Attack, PlayerRole.Defender, DiceModificationTimingType.Opposite, typeof(DiceModificationAttackByDefenderSubphase)),
            new DiceModificationStep(CombatStep.Attack, PlayerRole.Attacker, DiceModificationTimingType.Normal, typeof(DiceModificationAttackByAttackerSubphase)),
            new FinishDiceRollStep(typeof(AttackDiceRollCombatSubPhase)),
            new StartDiceRollStep(typeof(DefenseDiceRollCombatSubPhase)),
            new DiceModificationStep(CombatStep.Defence, PlayerRole.Defender, DiceModificationTimingType.AfterRolled, typeof(DiceModificationDefenseByAttackerSubphase)),
            new DiceModificationStep(CombatStep.Defence, PlayerRole.Attacker, DiceModificationTimingType.Opposite, typeof(DiceModificationDefenseByAttackerSubphase)),
            new DiceModificationStep(CombatStep.Defence, PlayerRole.Defender, DiceModificationTimingType.Normal, typeof(DiceModificationDefenseByDefenderSubphase)),
            new DiceModificationStep(CombatStep.CompareResults, PlayerRole.Attacker, DiceModificationTimingType.CompareResults, typeof(DiceModificationCompareResultsByAttackerSubphase)),
            new DiceModificationStep(CombatStep.CompareResults, PlayerRole.Defender, DiceModificationTimingType.CompareResults, typeof(DiceModificationCompareResultsByDefenderSubphase)),
            new FinishDiceRollStep(typeof(DefenseDiceRollCombatSubPhase)),
            new CompareAndDealDamageDiceStep(),
        };
    }

    public void StartAttack()
    {
        if (!Combat.SkipAttackDiceRollsAndHit)
        {
            Next();
        }
        else
        {
            GoDirectlyToCompareResults();
        }
    }

    public void Next()
    {
        CurrentDiceModificationStep = DiceModificationSteps.FirstOrDefault(n => !n.IsExecuted);
        if (CurrentDiceModificationStep != null)
        {
            CurrentDiceModificationStep.Start();
        }
        else
        {
            Combat.AfterAllDiceModificationsAreDone();
        }
    }

    public void UseDiceModification(string diceModificationName)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;
        Tooltips.EndTooltip();

        if (AvailableDiceModifications.ContainsKey(diceModificationName))
        {
            GenericAction diceModification = AvailableDiceModifications[diceModificationName];

            Selection.ActiveShip.AddAlreadyUsedDiceModification(diceModification);

            diceModification.ActionEffect(
                delegate {
                    Combat.CurrentDiceRoll.MarkAsModifiedBy(diceModification.HostShip.Owner.PlayerNo);
                    ReplaysManager.ExecuteWithDelay(Combat.DiceModifications.RefreshButtonsList);
                }
            );
        }
        else if (diceModificationName == "OK")
        {
            ReplaysManager.ExecuteWithDelay(Phases.CurrentSubPhase.Next);
        }
        else
        {
            Messages.ShowError("ERROR: Dice Modification is not found: " + diceModificationName);
        }
    }

    public void ConfirmDiceResults()
    {
        HideAllButtons();
        CurrentDiceModificationStep.WhenFinish();
    }

    public void RefreshButtonsList()
    {
        (CurrentDiceModificationStep as DiceModificationStep).ShowDiceModifications();
    }

    public static GameCommand GenerateDiceModificationCommand(string diceModificationName)
    {
        JSONObject parameters = new JSONObject();
        string diceModificationNameFixed = diceModificationName.Replace('"', '_');
        parameters.AddField("name", diceModificationNameFixed);
        return GameController.GenerateGameCommand(
            GameCommandTypes.DiceModification,
            Phases.CurrentSubPhase.GetType(),
            parameters.ToString()
        );
    }

    // Skip dice - directly to compare results

    private static void GoDirectlyToCompareResults()
    {
        CreateEmptyAttackDiceRoll();
        Phases.StartTemporarySubPhaseOld(typeof(CompareResultsSubPhase).Name, typeof(CompareResultsSubPhase));
        Combat.AttackHit();
    }

    private static void CreateEmptyAttackDiceRoll()
    {
        Combat.DiceRollAttack = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Combat, Combat.Attacker.Owner.PlayerNo);
    }
}
