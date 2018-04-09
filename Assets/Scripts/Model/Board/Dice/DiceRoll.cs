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

    public int Number { get; private set; }

    private DelegateDiceroll callBack;

    private bool isRolling;

    public DieSide[] ResultsArray
    {
        get { return this.DiceList.Select(n => n.Side).ToArray();}
    }

    public DiceRoll(DiceKind type, int number, DiceRollCheckType checkType)
    {
        Type = type;
        Number = number;
        CheckType = checkType;

        if (checkType != DiceRollCheckType.Virtual) SetSpawningPoint();

        GenerateDiceRoll();
    }

    private void GenerateDiceRoll()
    {
        DiceList = new List<Die>();
        for (int i = 0; i < Number; i++)
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

    public int Successes
    {
        get
        {
            return DiceList.Count(n => ((n.Side == DieSide.Success) || (n.Side == DieSide.Crit)));
        }
        private set { }
    }

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

    public int SelectedCount
    {
        get { return DiceList.Count(n => (n.IsSelected)); }
        private set { }
    }

    public void Roll(DelegateDiceroll callBack)
    {
        DiceRoll.CurrentDiceRoll = this;

        this.callBack = callBack;

        if (!Network.IsNetworkGame)
        {
            foreach (Die die in DiceList)
            {
                die.RandomizeRotation();
            }
            RollPreparedDice();
        }
        else
        {
            if (DebugManager.DebugNetwork) UI.AddTestLogEntry("DiceRoll.Roll");
            Network.GenerateRandom(new Vector2(0, 360), DiceList.Count * 3, SetDiceInitialRotation, RollPreparedDice);
        }
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
        foreach (Die die in DiceList.Where(n => n.IsSelected))
        {
            die.Reroll();
        }

        CalculateResults();
    }

    public void RerollSelected(DelegateDiceroll callBack)
    {
        this.callBack = callBack;

        if (!Network.IsNetworkGame)
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
            if (DebugManager.DebugNetwork) UI.AddTestLogEntry("DiceRoll.SyncSelectedDice");
            Network.SyncSelectedDiceAndReroll();
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

    public void ApplyEvade()
    {
        AddDice(DieSide.Success).ShowWithoutRoll();

        OrganizeDicePositions();
        UpdateDiceCompareHelperPrediction();
    }

	public void Change(DieSide oldSide, DieSide newSide, int count)
	{
		for (int i = 0; i < count; i++) {
			ChangeDice (oldSide, newSide, true);
		}

		UpdateDiceCompareHelperPrediction ();
	}

    public void ChangeOne(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        ChangeDice(oldSide, newSide, true, cannotBeRerolled, cannotBeModified);
        UpdateDiceCompareHelperPrediction();
    }

    public void ChangeAll(DieSide oldSide, DieSide newSide)
    {
        ChangeDice(oldSide, newSide, false);
        UpdateDiceCompareHelperPrediction();
    }

    private void ChangeDice(DieSide oldSide, DieSide newSide, bool onlyOne, bool cannotBeRerolled = false, bool cannotBeModified = false)
    {
        OrganizeDicePositions();
        foreach (Die die in DiceList)
        {
            if (die.Side == oldSide)
            {
                die.SetSide(newSide);
                die.SetModelSide(newSide);
                if (cannotBeRerolled) die.IsRerolled = true;
                if (cannotBeModified) die.CannotBeModified = true;
                if (onlyOne) return;
            }
        }
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

        // TODO: Rewrite
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Wait(1, StartWaitingForFinish);
        Game.Wait(5, CalculateWaitedResults);
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

            callBack(this);
        }
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
            DiceList[i].SetPosition(FinalPositionPoint.position + DiceManager.DicePositions[DiceList.Count-1][i]);
            if (DiceList[i].IsDiceFaceVisibilityWrong())
            {
                DiceList[i].SetModelSide(DiceList[i].Side);
            }
        }
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

    private void UpdateDiceCompareHelperPrediction()
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
        if (!Network.IsNetworkGame)
        {
            newDie.RandomizeRotation();
            BeforeRollAdditionalPreparedDice();
        }
        else
        {
            Network.GenerateRandom(new Vector2(0, 360), 3, SetAdditionalDiceInitialRotation, BeforeRollAdditionalPreparedDice);
        }

        /*Combat.Defender.CallDiceAboutToBeRolled();
        Triggers.ResolveTriggers(TriggerTypes.OnDiceAboutToBeRolled, delegate
        {
            Combat.CurrentDiceRoll.RollAdditionalDice(1);
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            callBack();
        });*/
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
        if (!Network.IsNetworkGame)
        {
            UnblockButtons();
        }
        else
        {
            Network.SyncDiceRollInResults();
        }
    }

    private void ToggleDiceModificationsPanel(bool isActive)
    {
        GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").gameObject.SetActive(isActive);

        if (isActive)
        {
            Combat.ToggleConfirmDiceResultsButton(true);

            // No branch for opposite dice modifications?
            Combat.ShowDiceModificationButtons();
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
}
