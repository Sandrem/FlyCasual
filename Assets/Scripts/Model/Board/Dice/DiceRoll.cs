using GameCommands;
using Editions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;

public partial class DiceRoll
{
    // INITIALIZE

    public DiceRoll(DiceKind type, int countOfInitialRoll, DiceRollCheckType checkType, PlayerNo owner = PlayerNo.PlayerNone)
    {
        Type = type;
        CountOfInitialRoll = countOfInitialRoll;
        CheckType = checkType;
        Owner = owner;

        if (checkType != DiceRollCheckType.Virtual) { SetDiceSpawningPoint(); }

        GenerateDiceRoll();
    }

    private void SetDiceSpawningPoint()
    {
        SpawningPoint = (CheckType == DiceRollCheckType.Combat) ? GameManagerScript.Instance.PrefabsList.CombatDiceSpawningPoint : GameManagerScript.Instance.PrefabsList.CheckDiceSpawningPoint;
        FinalPositionPoint = (CheckType == DiceRollCheckType.Combat) ? GameManagerScript.Instance.PrefabsList.CombatDiceField : GameManagerScript.Instance.PrefabsList.CheckDiceField;
    }

    private void GenerateDiceRoll()
    {
        for (int i = 0; i < CountOfInitialRoll; i++) AddDice();
    }

    public Die AddDice(DieSide side = DieSide.Unknown)
    {
        Die newDice = new Die(this, this.Type, side);
        DiceList.Add(newDice);
        return newDice;
    }

    // ROLL

    public void Roll(DelegateDiceroll callBack)
    {
        DiceRoll.CurrentDiceRoll = this;
        this.CallBack = callBack;

        if (!ShouldSkipToSync())
        {
            foreach (Die die in DiceList) die.RandomizeRotation();
            RollPreparedDice();
        }
        else
        {
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceResults();
        }
    }

    private void RollPreparedDice()
    {
        foreach (Die die in DiceList) die.Roll();
        GameManagerScript.Instance.StartCoroutine(CalculateResultsCororutine());
    }

    public IEnumerator CalculateResultsCororutine()
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        yield return new WaitForSeconds(1);
        yield return CheckDiceMovementFinish();
        yield return new WaitForSeconds(1);
        CalculateWaitedResults();
    }

    private IEnumerator CheckDiceMovementFinish()
    {
        int diceStillRolling = DiceList.Count;
        foreach (var die in DiceList) if (die.IsModelRollingFinished()) diceStillRolling--;
        yield return diceStillRolling == 0;
    }

    private void CalculateWaitedResults()
    {
        foreach (Die die in DiceList)
        {
            die.Model.GetComponentInChildren<Rigidbody>().isKinematic = true;

            DieSide face = die.GetModelFace();
            die.TrySetSide(face);

            if (die.IsWaitingForNewResult)
            {
                die.IsWaitingForNewResult = false;
                DiceManager.CallDiceResult(this, new DieResultEventArg(Owner, Type, die.Side));
            }
        }

        if (IsDiceFacesVisibilityWrong())
        {
            OrganizeDicePositions();
        }
        else
        {
            UpdateDiceCompareHelperPrediction();
        }

        Roster.GetPlayer(PlayerNo.Player1).SyncDiceResults(); // Syrver synchs dice
    }

    public static void SyncDiceResults(List<DieSide> sides)
    {
        bool wasFixed = false;

        for (int i = 0; i < DiceRoll.CurrentDiceRoll.DiceList.Count; i++)
        {
            Die die = DiceRoll.CurrentDiceRoll.DiceList[i];

            if (die.Model == null) die.ShowWithoutRoll();

            if (die.Side != sides[i])
            {
                die.TrySetSide(sides[i]);
                die.SetModelSide(sides[i]);

                wasFixed = true;
            }
        }

        if (wasFixed)
        {
            DiceRoll.CurrentDiceRoll.OrganizeDicePositions();
        }
        else
        {
            DiceRoll.CurrentDiceRoll.UpdateDiceCompareHelperPrediction();
        }

        ReplaysManager.ExecuteWithDelay(CurrentDiceRoll.ExecuteCallback);
    }

    private void ExecuteCallback()
    {
        CallBack(this);
    }

    // ROLL IN

    public void RollInDice(DelegateDiceroll callBack)
    {
        CallBack = callBack;
        Die newDie = AddDice();

        if (!ShouldSkipToSync())
        {
            newDie.RandomizeRotation();
            Selection.ActiveShip.CallDiceAboutToBeRolled(RollAdditionalPreparedDice);
        }
        else
        {
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceResults();
        }
    }

    private void RollAdditionalPreparedDice()
    {
        foreach (Die die in DiceList)
        {
            if (die.Model == null || !die.Model.activeSelf) die.Roll();
        }

        GameManagerScript.Instance.StartCoroutine(CalculateResultsCororutine());
    }

    // REROLL

    public void RerollSelected(DelegateDiceroll callBack)
    {
        DiceRoll.CurrentDiceRoll = this;
        this.CallBack = callBack;

        if (!ShouldSkipToSync())
        {
            foreach (Die die in DiceList) if (die.IsSelected) die.RandomizeRotation();
            RerollPreparedDice();
        }
        else
        {
            CurrentDiceRoll.DeselectAll();
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceResults();
        }
    }

    private void RerollPreparedDice()
    {
        DiceWereSelectedForRerollCount = SelectedCount;
        DiceRerolled = DiceList.Where(n => n.IsSelected).ToList();
        foreach (Die die in DiceRerolled) die.Reroll();
        GameManagerScript.Instance.StartCoroutine(CalculateResultsCororutine());
    }

    public void ToggleRerolledLocks(bool isActive)
    {
        foreach (var dice in DiceList) dice.ToggleRerolledLock(isActive);
    }

    // DICE MODIFICATIONS

    public void ApplyEvade()
    {
        Edition.Current.EvadeDiceModification(this);
        OrganizeDicePositions();
    }

    public void AddDiceAndShow(DieSide dieSide)
    {
        AddDice(dieSide).ShowWithoutRoll();
        OrganizeDicePositions();
    }

    public int Change(DieSide oldSide, DieSide newSide, int count = 0, bool cannotBeRerolled = false, bool cannotBeModified = false)
	{
        var changedDiceCount = 0;
        if (count == 0) // = change all
        {
            changedDiceCount += ChangeAll(oldSide, newSide, cannotBeRerolled, cannotBeModified);
        }
        else
        {
            changedDiceCount = Math.Min(count, DiceList.Count(n => n.Side == oldSide));
            for (int i = 0; i < changedDiceCount; i++) ChangeOne(oldSide, newSide, cannotBeRerolled, cannotBeModified);
		}

        OrganizeDicePositions();
        return changedDiceCount;
    }

    public int ChangeAll(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        int changedCount = ChangeDice(oldSide, newSide, false, cannotBeRerolled, cannotBeModified);
        return changedCount;
    }

    public void ChangeOne(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        ChangeDice(oldSide, newSide, onlyOne: true, cannotBeRerolled, cannotBeModified);
    }

    public void ChangeWorstResultTo(DieSide newSide)
    {
        ChangeDice(WorstResult, newSide, true);
    }

    private int ChangeDice(DieSide oldSide, DieSide newSide, bool onlyOne, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        var changedDiceCount = 0;
        foreach (Die die in DiceList)
        {
            if (die.Side == oldSide)
            {
                if (die.TrySetSide(newSide, isInitial: false))
                {
                    die.SetModelSide(newSide);
                    changedDiceCount++;

                    UpdateDiceCompareHelperPrediction();

                    if (cannotBeRerolled) die.IsRerolled = true;
                    if (cannotBeModified) die.CannotBeModified = true;
                    if (onlyOne) break;
                }
            }
        }

        OrganizeDicePositions();

        return changedDiceCount;
    }

    public DieSide FindDieToChange(DieSide destinationSide)
    {
        if (DiceList.Count <= 0)
        {
            Messages.ShowErrorToHuman("There are no dice available to be changed");
            return DieSide.Unknown;
        }

        if (destinationSide != DieSide.Blank)
        {
            if (Blanks > 0) return DieSide.Blank;
            if (Focuses > 0) return DieSide.Focus;
            if (Successes > 0) return DieSide.Success;
            if (CriticalSuccesses > 0) return DieSide.Crit;
        }
        else
        {   // Asking for blanks. We could put a switch case here to account for different CheckType situations.
            // This currently handles CheckType == Check well, which is I think where most people would ask for a blank.
            if (Type == DiceKind.Attack)
            {
                if (CriticalSuccesses > 0) return DieSide.Crit;
                if (Successes > 0) return DieSide.Success;
                if (Focuses > 0) return DieSide.Focus;
                if (Blanks > 0) return DieSide.Blank;
            }
            else
            {
                if (Focuses > 0) return DieSide.Focus;
                if (Successes > 0) return DieSide.Success;
                if (Blanks > 0) return DieSide.Blank;
            }
        }
        return DieSide.Unknown; // We never should get here
    }

    // CANCELLATION

    public Dictionary<string, int> CancelHitsByDefence(int countToCancel, bool dryRun = false)
    {
        Dictionary<string, int> results = new Dictionary<string, int>
        {
            ["crits"] = 0,
            ["hits"] = 0
        };

        for (int i = 0; i < countToCancel; i++)
        {
            DieSide result = CancelHit(isCancelByDefenceDice:true, dryRun);
            switch (result)
            {
                case DieSide.Success: { results["hits"]++; break; }
                case DieSide.Crit: { results["crits"]++; break; }
            }
        }
        return results;
    }

    public void CancelHitsSpecial(int numToCancel)
    {
        for (int i = 0; i < numToCancel; i++) CancelHit(isCancelByDefenceDice: false, dryRun: false);
    }

    private DieSide CancelHit(bool isCancelByDefenceDice, bool dryRun)
    {;
        DieSide cancelFirst = (!CancelCritsFirst) ? DieSide.Success : DieSide.Crit;
        DieSide cancelLast = (!CancelCritsFirst) ? DieSide.Crit : DieSide.Success;
        DieSide cancelResult = DieSide.Unknown;

        if (!CancelType(cancelFirst, isCancelByDefenceDice, dryRun))
        {
            if (CancelType(cancelLast, isCancelByDefenceDice, dryRun))
            {
                cancelResult = cancelLast;
            }
        }
        else
        {
            cancelResult = cancelFirst;
        }
        return cancelResult;
    }

    private bool CancelType(DieSide type, bool CancelByDefence, bool dryRun)
    {
        bool found = false;
        foreach (Die die in DiceList)
        {
            if (die.Side == type)
            {
                //Cancel dice if it's not a defence cancel or it is and the die is cancellable
                if ((!CancelByDefence) || (!die.IsUncancelable))
                {
                    die.Cancel();
                    found = true;
                    return found;
                }
            }
        }
        return found;
    }

    public void RemoveAllFailures()
    {
        List<Die> diceCopy = new List<Die>(DiceList);
        foreach (Die die in diceCopy) if (die.IsFailure) DiceList.Remove(die);
    }

    // Used to clean the diceboard before adding other dice [ Accuracy corrector ]
    public void RemoveAll()
    {
        RemoveDiceModels();
        DiceList = new List<Die>();
    }

    public void RemoveDiceModels()
    {
        foreach (Die die in DiceList) die.RemoveModel();
    }

    public bool RemoveType(DieSide type)
    {
        // Select a die that matches the type, prioritize those that aren't uncancellable
        Die dieToCancel = DiceList
            .OrderBy(d => d.IsUncancelable)
            .FirstOrDefault(d => d.Side == type);

        if (dieToCancel != null)
        {
            dieToCancel.Cancel();
            dieToCancel.RemoveModel();
            DiceList.Remove(dieToCancel);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CancelAllResults()
    {
        List<Die> diceListCopy = new List<Die>(DiceList);
        foreach (var die in diceListCopy) die.Cancel();
    }

    // VIEW

    public void OrganizeDicePositions()
    {
        for (int i = 0; i < DiceList.Count; i++)
        {
            if (DiceList[i].Model == null) continue;

            DiceList[i].SetPosition(FinalPositionPoint.position + DiceManager.DicePositions[DiceList.Count-1][i]);
            if (DiceList[i].IsDiceFaceVisibilityWrong()) DiceList[i].SetModelSide(DiceList[i].Side);
        }

        UpdateDiceCompareHelperPrediction();
    }

    public void UpdateDiceCompareHelperPrediction()
    {
        if (DiceCompareHelper.currentDiceCompareHelper != null && DiceCompareHelper.currentDiceCompareHelper.IsActive())
        {
            DiceCompareHelper.currentDiceCompareHelper.ShowCancelled(this);
        }
    }

    public bool IsDiceFacesVisibilityWrong()
    {
        foreach (var dice in DiceList) if (dice.IsDiceFaceVisibilityWrong()) return true;
        return false;
    }

    // SELECTION

    public void SelectAll()
    {
        foreach (var dice in DiceList) dice.ToggleSelected(true);
    }

    private void DeselectAll()
    {
        foreach (var dice in DiceList) dice.ToggleSelected(false);
    }

    private bool CanDieBeSelected(Die die, ref string error)
    {
        bool canSelect = true;

        if (die.CannotBeModified)
        {
            canSelect = false;
            error = "This die cannot be modified";
        }
        else if (die.IsRerolled)
        {
            canSelect = false;
            error = "Each die can only be re-rolled once";
        }
        else if (DiceRerollManager.CurrentDiceRerollManager.NumberOfDiceCanBeRerolled == SelectedCount)
        {
            canSelect = false;
            error = "Only " + DiceRerollManager.CurrentDiceRerollManager.NumberOfDiceCanBeRerolled + " dice can be selected";
        }
        else if (!DiceRerollManager.CurrentDiceRerollManager.SidesCanBeRerolled.Contains(die.Side))
        {
            canSelect = false;
            error = "Dice with this result cannot be rerolled";
        }

        Selection.ActiveShip.TrySelectDie(die, ref canSelect);
        return canSelect;
    }

    public void SelectBySides(List<DieSide> dieSides, int maxCanBeSelected)
    {
        DeselectAll();

        int alreadySelected = 0;

        if (alreadySelected < maxCanBeSelected)
        {
            foreach (var dieSide in dieSides)
            {
                //from blanks to focuses
                foreach (var die in DiceList)
                {
                    if (die.Side == dieSide)
                    {
                        string selectionErrors = "";
                        bool canSelect = CanDieBeSelected(die, ref selectionErrors);
                        if (canSelect)
                        {
                            die.ToggleSelected(true);
                            alreadySelected++;
                        }
                        if (alreadySelected == maxCanBeSelected)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    public void TrySelectDiceByModel(GameObject diceModel)
    {
        foreach (var die in DiceList)
        {
            if (die.Model.name == diceModel.transform.parent.name)
            {
                if (die.IsSelected)
                {
                    die.ToggleSelected(false);
                    return;
                }

                // Check If the Die Is Selectable. Show Error Message if exists.
                string errors = "";
                bool canSelect = CanDieBeSelected(die, ref errors);
                if (canSelect) die.ToggleSelected(true);
                else if (errors.Length > 0) Messages.ShowErrorToHuman(errors);
            }
        }
    }

    public static GameCommand GenerateSyncDiceCommand()
    {
        JSONObject[] diceResultArray = new JSONObject[DiceRoll.CurrentDiceRoll.DiceList.Count];
        for (int i = 0; i < DiceRoll.CurrentDiceRoll.DiceList.Count; i++)
        {
            DieSide side = DiceRoll.CurrentDiceRoll.DiceList[i].Side;
            string sideName = side.ToString();
            JSONObject sideJson = new JSONObject();
            sideJson.AddField("side", sideName);
            diceResultArray[i] = sideJson;
        }
        JSONObject dieSides = new JSONObject(diceResultArray);
        JSONObject parameters = new JSONObject();
        parameters.AddField("sides", dieSides);

        return GameController.GenerateGameCommand(
            GameCommandTypes.SyncDiceResults,
            Phases.CurrentSubPhase.GetType(),
            parameters.ToString()
        );
    }

    public void MarkAsModifiedBy(PlayerNo playerNo)
    {
        if (!Combat.CurrentDiceRoll.ModifiedByPlayers.Contains(playerNo))
        {
            Combat.CurrentDiceRoll.ModifiedByPlayers.Add(playerNo);
        }
    }
}
