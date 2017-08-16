using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public int Number { get; private set; }

    private DelegateDiceroll callBack;

    public DiceRoll(DiceKind type, int number)
    {
        Type = type;
        Number = number;
        GenerateDiceRoll(type, number);
    }

    private void GenerateDiceRoll(DiceKind type, int number)
    {
        DiceList = new List<Dice>();
        for (int i = 0; i < number; i++)
        {
            DiceList.Add(new Dice(type));
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
        Dice addEvade = new Dice(DiceKind.Defence, DiceSide.Success);
        DiceList.Add(addEvade);
        addEvade.NoRoll();

        OrganizeDicePositions();
        UpdateDiceCompareHelperPrediction();
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

    private void AddDice(DiceSide side = DiceSide.Unknown)
    {
        DiceList.Add(new Dice(Type, side));
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
        List<Dice> dices = new List<Dice>();
        dices.AddRange(DiceList);

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

    public void CalculateResults()
    {
        // TODO: Rewrite
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Movement.FuncsToUpdate.Add(CheckDiceMovementFinish);
    }

    private bool CheckDiceMovementFinish()
    {
        bool result = false;

        int dicesStillRolling = DiceList.Count;
        foreach (var dice in DiceList)
        {
            if (dice.IsModelRollingFinished()) dicesStillRolling--;
        }

        if (dicesStillRolling == 0)
        {
            CalculateWaitedResults();
            callBack(this);
            result = true;
        }

        return result;
    }

    private void CalculateWaitedResults()
    {
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
            DiceList[i].SetPosition(DicesManager.DiceField.position + DicesManager.DicePositions[DiceList.Count-1][i]);
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
