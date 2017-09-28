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

    public int NotRerolled
    {
        get { return DiceList.Count(n => (n.IsRerolled == false)); }
        private set { }
    }

    public void Roll(DelegateDiceroll callBack)
    {
        this.callBack = callBack;

        foreach (Die dice in DiceList)
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

    public void ChangeOne(DieSide oldSide, DieSide newSide, bool cannotBeRerolled = false)
    {
        ChangeDice(oldSide, newSide, true, cannotBeRerolled);
        UpdateDiceCompareHelperPrediction();
    }

    public void ChangeAll(DieSide oldSide, DieSide newSide)
    {
        ChangeDice(oldSide, newSide, false);
        UpdateDiceCompareHelperPrediction();
    }

    private void ChangeDice(DieSide oldSide, DieSide newSide, bool onlyOne, bool cannotBeRerolled = false)
    {
        OrganizeDicePositions();
        foreach (Die die in DiceList)
        {
            if (die.Side == oldSide)
            {
                die.SetSide(newSide);
                die.SetModelSide(newSide);
                if (cannotBeRerolled) die.IsRerolled = true;
                if (onlyOne) return;
            }
        }
    }

    private void CancelHit()
    {
        if (!CancelType(DieSide.Success))
        {
            CancelType(DieSide.Crit);
        }
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

    private bool CancelType(DieSide type)
    {
        bool found = false;
        foreach (Die dice in DiceList)
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
        Game.Movement.FuncsToUpdate.Add(CheckDiceMovementFinish);
        Game.Wait(5, CalculateWaitedResults);
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

                if (die.IsRerolled)
                {
                    Messages.ShowErrorToHuman("Dice can be rerolled only once");
                    return;
                }

                if (DiceRerollManager.currentDiceRerollManager.NumberOfDiceCanBeRerolled == GetSelectedNumber())
                {
                    Messages.ShowErrorToHuman("Only " + DiceRerollManager.currentDiceRerollManager.NumberOfDiceCanBeRerolled + " dice can be selected");
                    return;
                }

                if (!DiceRerollManager.currentDiceRerollManager.SidesCanBeRerolled.Contains(die.Side))
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
        if (DiceCompareHelper.currentDiceCompareHelper != null)
        {
            DiceCompareHelper.currentDiceCompareHelper.ShowCancelled(this);
        }
    }

}
