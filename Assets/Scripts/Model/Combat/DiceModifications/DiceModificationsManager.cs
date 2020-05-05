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
            // new DiceModificationStep(CombatStep.Attack, PlayerRole.Defender, typeof(DiceModificationAttackByDefenderSubphase)),
            new DiceModificationStep(CombatStep.Attack, PlayerRole.Attacker, typeof(DiceModificationAttackByAttackerSubphase)),
            new FinishDiceRollStep(typeof(AttackDiceRollCombatSubPhase)),
            new StartDiceRollStep(typeof(DefenseDiceRollCombatSubPhase)),
            // new DiceModificationStep(CombatStep.Defense, PlayerRole.Attacker, typeof(DiceModificationDefenseByAttackerSubphase)),
            new DiceModificationStep(CombatStep.Defence, PlayerRole.Defender, typeof(DiceModificationDefenseByDefenderSubphase)),
            new FinishDiceRollStep(typeof(DefenseDiceRollCombatSubPhase)),
            // new DiceModificationStep(CombatStep.CompareResults, PlayerRole.Defender, typeof(DiceModificationCompareResultsByDefenderSubphase)),
            // new DiceModificationStep(CombatStep.CompareResults, PlayerRole.Attacker, typeof(DiceModificationCompareResultsByAttackerSubphase))
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

    private static void GoDirectlyToCompareResults()
    {
        CreateEmptyAttackDiceRoll();
        Phases.StartTemporarySubPhaseOld(typeof(CompareResultsSubPhase).Name, typeof(CompareResultsSubPhase));
        Combat.AttackHit();
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

    public void ConfirmDiceResults()
    {
        HideDiceModificationsButtonsList();
        CurrentDiceModificationStep.Finish();
    }

    private static void CreateEmptyAttackDiceRoll()
    {
        Combat.DiceRollAttack = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Combat, Combat.Attacker.Owner.PlayerNo);
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

    public void RefreshButtonsList()
    {
        (CurrentDiceModificationStep as DiceModificationStep).ShowDiceModifications();
    }
}
