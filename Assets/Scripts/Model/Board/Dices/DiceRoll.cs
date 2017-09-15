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
    public List<Dice> DiceList
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

    public Transform SpawningPoint;
    public Transform FinalPositionPoint;

    public int Number { get; private set; }

    private DelegateDiceroll callBack;

    private bool isRolling;

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
        DiceList = new List<Dice>();
        for (int i = 0; i < Number; i++)
        {
            AddDice();
        }
    }

    public Dice AddDice(DiceSide side = DiceSide.Unknown)
    {
        Dice newDice = new Dice(this, Type, side);
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
            return DiceList.Count(n => ((n.Side == DiceSide.Success) || (n.Side == DiceSide.Crit)));
        }
        private set { }
    }

    public int RegularSuccesses
    {
        get
        {
            return DiceList.Count(n => (n.Side == DiceSide.Success));
        }
        private set { }
    }

    public int CriticalSuccesses
    {
        get
        {
            return DiceList.Count(n => (n.Side == DiceSide.Crit));
        }
        private set { }
    }

    public int Focuses
    {
        get { return DiceList.Count(n => (n.Side == DiceSide.Focus)); }
        private set { }
    }

    public int FocusesNotRerolled
    {
        get { return DiceList.Count(n => ((n.Side == DiceSide.Focus) && (n.IsRerolled == false))); }
        private set { }
    }

    public int Blanks
    {
        get { return DiceList.Count(n => (n.Side == DiceSide.Blank)); }
        private set { }
    }

    public int BlanksNotRerolled
    {
        get { return DiceList.Count(n => ((n.Side == DiceSide.Blank) && (n.IsRerolled == false))); }
        private set { }
    }

    public void Roll(DelegateDiceroll callBack)
    {
        this.callBack = callBack;

        foreach (Dice dice in DiceList)
        {
            dice.Roll();
        }

        CalculateResults();
    }

    public void RerollSelected(DelegateDiceroll callBack)
    {
        this.callBack = callBack;

        foreach (var dice in DiceList)
        {
            if (dice.IsSelected)
            {
                dice.Reroll();
            }
        }

        CalculateResults();
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
        ChangeAll(DiceSide.Focus, DiceSide.Success);

        OrganizeDicePositions();
    }

    public void ApplyEvade()
    {
        AddDice(DiceSide.Success).ShowWithoutRoll();

        OrganizeDicePositions();
        UpdateDiceCompareHelperPrediction();
    }

	public void Change(DiceSide oldSide, DiceSide newSide, int count)
	{
		for (int i = 0; i < count; i++) {
			ChangeDice (oldSide, newSide, true);
		}

		UpdateDiceCompareHelperPrediction ();
	}

    public void ChangeOne(DiceSide oldSide, DiceSide newSide)
    {
        ChangeDice(oldSide, newSide, true);
        UpdateDiceCompareHelperPrediction();
    }

    public void ChangeAll(DiceSide oldSide, DiceSide newSide)
    {
        ChangeDice(oldSide, newSide, false);
        UpdateDiceCompareHelperPrediction();
    }

    private void ChangeDice(DiceSide oldSide, DiceSide newSide, bool onlyOne)
    {
        OrganizeDicePositions();
        foreach (Dice dice in DiceList)
        {
            if (dice.Side == oldSide)
            {
                dice.SetSide(newSide);
                dice.SetModelSide(newSide);
                if (onlyOne) return;
            }
        }
    }

    private void CancelHit()
    {
        if (!CancelType(DiceSide.Success))
        {
            CancelType(DiceSide.Crit);
        }
    }

    public void RemoveAllFailures()
    {
        List<Dice> dices = new List<Dice>(DiceList);

        foreach (Dice dice in dices)
        {
            if ((dice.Side == DiceSide.Blank) || (dice.Side == DiceSide.Focus))
            {
                DiceList.Remove(dice);
            }
        }
    }

    private bool CancelType(DiceSide type)
    {
        bool found = false;
        foreach (Dice dice in DiceList)
        {
            if (dice.Side == type)
            {
                dice.Cancel();
                found = true;
                return found;
            }
        }
        return found;
    }

    public void CancelHits(int numToCancel)
    {
        for (int i = 0; i < numToCancel; i++)
        {
            CancelHit();
        }
    }

    public void CancelAllResults()
    {
        List<Dice> dicesListCopy = new List<Dice>(DiceList);

        foreach (var dice in dicesListCopy)
        {
            dice.Cancel();
        }
    }

    public void CalculateResults()
    {
        isRolling = true;

        // TODO: Rewrite
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Movement.FuncsToUpdate.Add(CheckDiceMovementFinish);
        Game.Wait(5, CalculateWaitedResults);
    }

    private bool CheckDiceMovementFinish()
    {
        bool result = false;

        if (isRolling)
        {
            int dicesStillRolling = DiceList.Count;
            foreach (var dice in DiceList)
            {
                if (dice.IsModelRollingFinished()) dicesStillRolling--;
            }

            if (dicesStillRolling == 0)
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

            foreach (Dice dice in DiceList)
            {
                DiceSide face = dice.GetModelFace();
                dice.SetSide(face);
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
        foreach (Dice dice in DiceList)
        {
            dice.RemoveModel();
        }
    }

    public void OrganizeDicePositions()
    {
        for (int i = 0; i < DiceList.Count; i++)
        {
            DiceList[i].SetPosition(FinalPositionPoint.position + DicesManager.DicePositions[DiceList.Count-1][i]);
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

    public void SelectBySides(List<DiceSide> diceSides, int maxCanBeSelected)
    {
        DeselectDices();

        int alreadySelected = 0;

        if (alreadySelected < maxCanBeSelected)
        {
            foreach (var diceSide in diceSides)
            {
                //from blanks to focuses
                foreach (var dice in DiceList)
                {
                    if ((dice.Side == diceSide) && (!dice.IsRerolled))
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

    private void DeselectDices()
    {
        foreach (var dice in DiceList)
        {
            dice.ToggleSelected(false);
        }
    }

    public void TrySelectDiceByModel(GameObject diceModel)
    {
        foreach (var dice in DiceList)
        {
            if (dice.Model.name == diceModel.transform.parent.name)
            {
                if (dice.IsSelected)
                {
                    dice.ToggleSelected(false);
                    return;
                }

                if (dice.IsRerolled)
                {
                    Messages.ShowErrorToHuman("Dice can be rerolled only once");
                    return;
                }

                if (DiceRerollManager.currentDiceRerollManager.NumberOfDicesCanBeRerolled == GetSelectedNumber())
                {
                    Messages.ShowErrorToHuman("Only " + DiceRerollManager.currentDiceRerollManager.NumberOfDicesCanBeRerolled + " dices can be selected");
                    return;
                }

                if (!DiceRerollManager.currentDiceRerollManager.SidesCanBeRerolled.Contains(dice.Side))
                {
                    Messages.ShowErrorToHuman("Dices with this result cannot be rerolled");
                    return;
                }

                dice.ToggleSelected(true);
            }
        }
    }

    private void UpdateDiceCompareHelperPrediction()
    {
        if (DiceCompareHelper.currentDiceCompareHelper != null)
        {
            DiceCompareHelper.currentDiceCompareHelper.ShowCancelled(this);
        }
    }

}
