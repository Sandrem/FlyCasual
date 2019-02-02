using GameCommands;
using Editions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DiceRollCheckType
{
    Combat,
    Check,
    Virtual
}

public partial class DiceRoll
{
    public List<Die> DiceList
    {
        get;
        private set;
    }

    public DiceKind Type
    {
        get;
        private set;
    }

    public DiceRollCheckType CheckType
    {
        get;
        private set;
    }

    public static DiceRoll CurrentDiceRoll;

    public bool CancelCritsFirst;

    public Transform SpawningPoint;
    public Transform FinalPositionPoint;

    public int CountOfInitialRoll { get; private set; }
    public int Count { get { return DiceList.Count; } }

    private DelegateDiceroll callBack;

    private bool isRolling;

    public DieSide[] ResultsArray
    {
        get { return this.DiceList.Select(n => n.Side).ToArray();}
    }

    public DieSide WorstResult
    {
        get
        {
            if (Blanks > 0) return DieSide.Blank;
            if (Focuses > 0) return DieSide.Focus;
            if (RegularSuccesses > 0) return DieSide.Success;
            if (CriticalSuccesses > 0) return DieSide.Crit;

            return DieSide.Unknown;
        }
    }

    public DiceRoll(DiceKind type, int number, DiceRollCheckType checkType)
    {
        Type = type;
        CountOfInitialRoll = number;
        CheckType = checkType;

        if (checkType != DiceRollCheckType.Virtual) SetSpawningPoint();

        GenerateDiceRoll();
    }

    private void GenerateDiceRoll()
    {
        DiceList = new List<Die>();
        for (int i = 0; i < CountOfInitialRoll; i++)
        {
            AddDice();
        }
    }

    public Die AddDice(DieSide side = DieSide.Unknown)
    {
        Die newDice = new Die(this, Type, side);
        DiceList.Add(newDice);
        return newDice;
    }

    private void SetSpawningPoint()
    {
        //Temporary
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        switch (CheckType)
        {
            case DiceRollCheckType.Combat:
                SpawningPoint = Game.PrefabsList.CombatDiceSpawningPoint;
                FinalPositionPoint = Game.PrefabsList.CombatDiceField;
                break;
            case DiceRollCheckType.Check:
                SpawningPoint = Game.PrefabsList.CheckDiceSpawningPoint;
                FinalPositionPoint = Game.PrefabsList.CheckDiceField;
                break;
            default:
                break;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return DiceList.Count == 0;
        }
    }

    public int Successes { get { return DiceList.Count(n => ((n.Side == DieSide.Success) || (n.Side == DieSide.Crit))); } }

    public int Failures { get { return DiceList.Count(n => ((n.Side == DieSide.Blank) || (n.Side == DieSide.Focus))); } }

    public int SuccessesCancelable
    {
        get
        {
            return DiceList.Count(n => ((n.Side == DieSide.Success) || (n.Side == DieSide.Crit)) && (n.IsUncancelable == false));
        }
        private set { }
    }

    public int RegularSuccesses
    {
        get
        {
            return DiceList.Count(n => (n.Side == DieSide.Success));
        }
        private set { }
    }

    public int CriticalSuccesses
    {
        get
        {
            return DiceList.Count(n => (n.Side == DieSide.Crit));
        }
        private set { }
    }

    public int Focuses
    {
        get { return DiceList.Count(n => (n.Side == DieSide.Focus)); }
        private set { }
    }

    public int FocusesNotRerolled
    {
        get { return DiceList.Count(n => ((n.Side == DieSide.Focus) && (n.IsRerolled == false))); }
        private set { }
    }

    public int Blanks
    {
        get { return DiceList.Count(n => (n.Side == DieSide.Blank)); }
        private set { }
    }

    public int BlanksNotRerolled
    {
        get { return DiceList.Count(n => ((n.Side == DieSide.Blank) && (n.IsRerolled == false))); }
        private set { }
    }

    public int CanBeModified
    {
        get { return DiceList.Count(n => n.CannotBeModified == false); }
        private set { }
    }

    public int CannotBeModified
    {
        get { return DiceList.Count(n => n.CannotBeModified == true); }
        private set { }
    }

    public int NotRerolled
    {
        get { return DiceList.Count(n => (n.IsRerolled == false)); }
        private set { }
    }

    public List<Die> Selected
    {
        get { return DiceList.Where(n => (n.IsSelected)).ToList(); }
        private set { }
    }

    public int WasSelectedCount { get; private set; }

    public int SelectedCount
    {
        get { return DiceList.Count(n => (n.IsSelected)); }
        private set { }
    }

    public void Roll(DelegateDiceroll callBack)
    {
        DiceRoll.CurrentDiceRoll = this;

        this.callBack = callBack;

        if (!ShouldSkipToSync())
        {
            foreach (Die die in DiceList)
            {
                die.RandomizeRotation();
            }
            RollPreparedDice();
        }
        else
        {
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceResults();
        }
    }

    private bool ShouldSkipToSync()
    {
        return (ReplaysManager.Mode == ReplaysMode.Read) || (Network.IsNetworkGame && !Network.IsServer);
    }

    private void SetDiceInitialRotation(int[] randomHolder)
    {
        int counter = 0;
        foreach (Die die in DiceList)
        {
            die.SetInitialRotation(new Vector3(randomHolder[counter], randomHolder[counter+1], randomHolder[counter+2]));
            counter += 3;
        }
    }

    private void SetAdditionalDiceInitialRotation(int[] randomHolder)
    {
        int counter = 0;
        foreach (Die die in DiceList)
        {
            if (die.Model == null || !die.Model.activeSelf)
            {
                die.SetInitialRotation(new Vector3(randomHolder[counter], randomHolder[counter + 1], randomHolder[counter + 2]));
                counter += 3;
            }
        }
    }

    private void SetSelectedDiceInitialRotation(int[] randomHolder)
    {
        int counter = 0;
        foreach (Die die in DiceList.Where(n => n.IsSelected))
        {
            die.SetInitialRotation(new Vector3(randomHolder[counter], randomHolder[counter + 1], randomHolder[counter + 2]));
            counter += 3;
        }
    }

    private void RollPreparedDice()
    {
        foreach (Die die in DiceList)
        {
            die.Roll();
        }

        CalculateResults();
    }

    private void BeforeRollAdditionalPreparedDice()
    {
        Selection.ActiveShip.CallDiceAboutToBeRolled(RollAdditionalPreparedDice);
    }

    private void RollAdditionalPreparedDice()
    {
        foreach (Die die in DiceList)
        {
            if (die.Model == null || !die.Model.activeSelf) die.Roll();
        }

        CalculateResults();
    }

    private void RerollPreparedDice()
    {
        WasSelectedCount = SelectedCount;

        foreach (Die die in DiceList.Where(n => n.IsSelected))
        {
            die.Reroll();
        }

        CalculateResults();
    }

    public void RerollSelected(DelegateDiceroll callBack)
    {
        DiceRoll.CurrentDiceRoll = this;

        this.callBack = callBack;

        if (!ShouldSkipToSync())
        {
            foreach (Die die in DiceList)
            {
                if (die.IsSelected)
                {
                    die.RandomizeRotation();
                }
            }
            RerollPreparedDice();
        }
        else
        {
            CurrentDiceRoll.DeselectDice();

            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).SyncDiceResults();
        }
    }

    public void RandomizeAndRerollSelected()
    {
        if (DebugManager.DebugNetwork) UI.AddTestLogEntry("DiceRoll.RerollSelected");
        Network.GenerateRandom(new Vector2(0, 360), SelectedCount * 3, SetSelectedDiceInitialRotation, RerollPreparedDice);
    }

    public void ToggleRerolledLocks(bool isActive)
    {
        foreach (var dice in DiceList)
        {
            dice.ToggleRerolledLock(isActive);
        }
    }

    public void ApplyFocus()
    {
        ChangeAll(DieSide.Focus, DieSide.Success);

        OrganizeDicePositions();
    }

    public void ApplyCalculate()
    {
        ChangeOne(DieSide.Focus, DieSide.Success);

        OrganizeDicePositions();
    }

    public void ApplyEvade()
    {
        Edition.Current.EvadeDiceModification(this);

        OrganizeDicePositions();
    }

	public int Change(DieSide oldSide, DieSide newSide, int count, bool cannotBeRerolled = false, bool cannotBeModified = false)
	{
        var changedDiceCount = 0;
        if (count == 0) // change all
        {
            changedDiceCount += ChangeDice(oldSide, newSide, false, cannotBeRerolled, cannotBeModified);
        }
        else
        {
            count = Math.Min(count, DiceList.Count(n => n.Side == oldSide));
            for (int i = 0; i < count; i++)
            {
                changedDiceCount += ChangeDice(oldSide, newSide, true, cannotBeRerolled, cannotBeModified);
            }
		}

		UpdateDiceCompareHelperPrediction();
        return changedDiceCount;
    }

    public void ChangeOne(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        ChangeDice(oldSide, newSide, true, cannotBeRerolled, cannotBeModified);
        UpdateDiceCompareHelperPrediction();
    }

    public void ChangeWorstResultTo(DieSide newSide)
    {
        ChangeDice(WorstResult, newSide, true);
        UpdateDiceCompareHelperPrediction();
    }

    public void ChangeAll(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        ChangeDice(oldSide, newSide, false, cannotBeRerolled, cannotBeModified);
        UpdateDiceCompareHelperPrediction();
    }

    private int ChangeDice(DieSide oldSide, DieSide newSide, bool onlyOne, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        var changedDiceCount = 0;
        OrganizeDicePositions();
        foreach (Die die in DiceList)
        {
            if (die.Side == oldSide)
            {
                die.SetSide(newSide);
                die.SetModelSide(newSide);
                changedDiceCount++;
                if (cannotBeRerolled) die.IsRerolled = true;
                if (cannotBeModified) die.CannotBeModified = true;
                if (onlyOne) return changedDiceCount;
            }
        }

        return changedDiceCount;
    }

    private DieSide CancelHit(bool CancelByDefence, bool dryRun)
    {
        DieSide cancelFirst = DieSide.Unknown;
        DieSide cancelLast = DieSide.Unknown;
        DieSide cancelResult = DieSide.Unknown;

        if (!CancelCritsFirst)
        {
            cancelFirst = DieSide.Success;
            cancelLast = DieSide.Crit;
        }
        else
        {
            cancelFirst = DieSide.Crit;
            cancelLast = DieSide.Success;
        }

        if (!CancelType(cancelFirst, CancelByDefence, dryRun))
        {
            if (CancelType(cancelLast, CancelByDefence, dryRun))
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

    public void RemoveAllFailures()
    {
        List<Die> dice = new List<Die>(DiceList);

        foreach (Die die in dice)
        {
            if ((die.Side == DieSide.Blank) || (die.Side == DieSide.Focus))
            {
                DiceList.Remove(die);
            }
        }
    }

    //Used to clean the diceboard before adding other dice [ Accuracy corrector ]
    public void RemoveAll()
    {
        RemoveDiceModels();
        DiceList = new List<Die>();
    }

    public bool RemoveType(DieSide type)
    {
        // Select a die that matches the type, prioritize those that aren't uncancellable
        var die = this.DiceList
            .OrderBy(d => d.IsUncancelable)
            .FirstOrDefault(d => d.Side == type);
        if (die != null)
        {
            die.Cancel();
            die.RemoveModel();
            this.DiceList.Remove(die);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CancelType(DieSide type, bool CancelByDefence, bool dryRun)
    {
        bool found = false;
        foreach (Die die in this.DiceList)
        {
            if (die.Side == type)
            {
                //Cancel dice if it's not a defence cancel or it is and the die is cancellable
                if ((!CancelByDefence) || (!die.IsUncancelable)) {
                    die.Cancel();
                    found = true;
                    return found;
                }
            }
        }
        return found;
    }

    public void CancelHits(int numToCancel)
    {
        for (int i = 0; i < numToCancel; i++)
        {
            CancelHit(false, false); //Generic cancel, not a test
        }
    }

    public Dictionary<string,int> CancelHitsByDefence(int countToCancel, bool dryRun = false)
    {
        Dictionary<string, int> results = new Dictionary<string, int>();
        results["crits"] = 0;
        results["hits"] = 0;

        for (int i = 0; i < countToCancel; i++)
        {
            DieSide result = CancelHit(true, dryRun); //Cancel by defence dice 
            switch(result)
            {
                case DieSide.Success: { results["hits"]++; break; }
                case DieSide.Crit: { results["crits"]++; break; }                
            }
        }
        return results;
    }

    public void CancelAllResults()
    {
        List<Die> diceListCopy = new List<Die>(DiceList);

        foreach (var die in diceListCopy)
        {
            die.Cancel();
        }
    }

    public void CalculateResults()
    {
        isRolling = true;

        GameManagerScript.Wait(1, StartWaitingForFinish);
        GameManagerScript.Wait(5, CalculateWaitedResults);
    }

    private void StartWaitingForFinish()
    {
        // TODO: Rewrite
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Movement.FuncsToUpdate.Add(CheckDiceMovementFinish);
    }

    private bool CheckDiceMovementFinish()
    {
        bool result = false;

        if (isRolling)
        {
            int diceStillRolling = DiceList.Count;
            foreach (var die in DiceList)
            {
                if (die.IsModelRollingFinished()) diceStillRolling--;
            }

            if (diceStillRolling == 0)
            {
                CalculateWaitedResults();
                result = true;
            }
        }
        else
        {
            result = true;
        }

        return result;
    }

    private void CalculateWaitedResults()
    {
        if (isRolling)
        {
            isRolling = false;

            foreach (Die die in DiceList)
            {
                DieSide face = die.GetModelFace();
                die.SetSide(face);
            }

            if (IsDiceFacesVisibilityWrong())
            {
                OrganizeDicePositions();
            }

            UpdateDiceCompareHelperPrediction();

            Roster.GetPlayer(Players.PlayerNo.Player1).SyncDiceResults(); // Server synchs dice
        }
    }

    public static void SyncDiceResults(List<DieSide> sides)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        bool wasFixed = false;

        for (int i = 0; i < DiceRoll.CurrentDiceRoll.DiceList.Count; i++)
        {
            Die die = DiceRoll.CurrentDiceRoll.DiceList[i];

            if (die.Model == null) die.ShowWithoutRoll();

            if (die.Side != sides[i])
            {
                die.SetSide(sides[i]);
                die.SetModelSide(sides[i]);

                wasFixed = true;
            }
        }

        if (wasFixed) DiceRoll.CurrentDiceRoll.OrganizeDicePositions();

        ReplaysManager.ExecuteWithDelay(CurrentDiceRoll.ExecuteCallback);
    }

    public void ExecuteCallback()
    {
        callBack(this);
    }

    public void RemoveDiceModels()
    {
        foreach (Die die in DiceList)
        {
            die.RemoveModel();
        }
    }

    public void OrganizeDicePositions()
    {
        for (int i = 0; i < DiceList.Count; i++)
        {
            if (DiceList[i].Model == null) continue;

            DiceList[i].SetPosition(FinalPositionPoint.position + DiceManager.DicePositions[DiceList.Count-1][i]);
            if (DiceList[i].IsDiceFaceVisibilityWrong())
            {
                DiceList[i].SetModelSide(DiceList[i].Side);
            }
        }

        UpdateDiceCompareHelperPrediction();
    }

    public bool IsDiceFacesVisibilityWrong()
    {
        bool result = false;

        foreach (var dice in DiceList)
        {
            bool isDiceWrong = dice.IsDiceFaceVisibilityWrong();
            if (isDiceWrong)
            {
                result = isDiceWrong;
            }
        }

        return result;
    }

    private int GetSelectedNumber()
    {
        int result = 0;

        foreach (var dice in DiceList)
        {
            if (dice.IsSelected) result++;
        }

        return result;
    }

    public void SelectAll()
    {
        foreach (var dice in DiceList)
        {
            dice.ToggleSelected(true);
        }
    }

    public void SelectBySides(List<DieSide> dieSides, int maxCanBeSelected)
    {
        DeselectDice();

        int alreadySelected = 0;

        if (alreadySelected < maxCanBeSelected)
        {
            foreach (var dieSide in dieSides)
            {
                //from blanks to focuses
                foreach (var dice in DiceList)
                {
                    if ((dice.Side == dieSide) && (!dice.IsRerolled))
                    {
                        dice.ToggleSelected(true);
                        alreadySelected++;
                        if (alreadySelected == maxCanBeSelected)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    private void DeselectDice()
    {
        foreach (var dice in DiceList)
        {
            dice.ToggleSelected(false);
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

                if (die.CannotBeModified)
                {
                    Messages.ShowErrorToHuman("This die cannot be modified.");
                    return;
                }

                if (die.IsRerolled)
                {
                    Messages.ShowErrorToHuman("Dice can be rerolled only once");
                    return;
                }

                if (DiceRerollManager.CurrentDiceRerollManager.NumberOfDiceCanBeRerolled == GetSelectedNumber())
                {
                    Messages.ShowErrorToHuman("Only " + DiceRerollManager.CurrentDiceRerollManager.NumberOfDiceCanBeRerolled + " dice can be selected");
                    return;
                }

                if (!DiceRerollManager.CurrentDiceRerollManager.SidesCanBeRerolled.Contains(die.Side))
                {
                    Messages.ShowErrorToHuman("Dice with this result cannot be rerolled");
                    return;
                }

                die.ToggleSelected(true);
            }
        }
    }

    public void UpdateDiceCompareHelperPrediction()
    {
        if (DiceCompareHelper.currentDiceCompareHelper != null && DiceCompareHelper.currentDiceCompareHelper.IsActive())
        {
            DiceCompareHelper.currentDiceCompareHelper.ShowCancelled(this);
        }
    }

    public void RollInDice(Action callBack)
    {
        this.callBack = delegate { TryUnblockButtons(this); callBack(); };

        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer)) BlockButtons();

        Die newDie = AddDice();

        newDie.RandomizeRotation();
        BeforeRollAdditionalPreparedDice();
    }

    private void BlockButtons()
    {
        ToggleDiceModificationsPanel(false);
    }

    public void UnblockButtons()
    {
        ToggleDiceModificationsPanel(true);
    }

    public void TryUnblockButtons(DiceRoll diceRoll)
    {
        UnblockButtons();
    }

    private void ToggleDiceModificationsPanel(bool isActive)
    {
        GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").gameObject.SetActive(isActive);

        if (isActive)
        {
            Combat.ToggleConfirmDiceResultsButton(true);
            Combat.ShowDiceModificationButtons(DiceModificationTimingType.Normal);
        }
        else
        {
            Combat.HideDiceModificationButtons();
        }
    }

    public DieSide FindDieToChange(DieSide destinationSide)
    {
        if (DiceList.Count <= 0)
        {
            Messages.ShowErrorToHuman("No dice in this roll to change.");
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
            } else
            {
                if (Focuses > 0) return DieSide.Focus;
                if (Successes > 0) return DieSide.Success;
                if (Blanks > 0) return DieSide.Blank;
            }
        }
        return DieSide.Unknown; // We never should get here
    }

    public bool HasResult(DieSide side)
    {
        return DiceList.Any(n => n.Side == side);
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
}
