using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DiceRoll
{

    public DiceRoll(string type, int number)
    {
        Type = type;
        Number = number;
        GenerateDiceRoll(type, number);
    }

    private void GenerateDiceRoll(string type, int number)
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
            int result = 0;
            foreach (Dice dice in DiceList)
            {
                if ((dice.Side == DiceSide.Success) || (dice.Side == DiceSide.Crit))
                {
                    result++;
                }
            }
            return result;
        }
        private set { }
    }

    public int Focuses
    {
        get
        {
            int result = 0;
            foreach (Dice dice in DiceList)
            {
                if (dice.Side == DiceSide.Focus)
                {
                    result++;
                }
            }
            return result;
        }
        private set { }
    }

    public void Roll()
    {
        foreach (Dice dice in DiceList)
        {
            dice.Roll();
        }
    }

    //Change to enum
    public void Reroll(string type)
    {
        foreach (Dice dice in DiceList)
        {
            if (type == "all")
                dice.Reroll();
            if (type == "blank")
            {
                if (dice.Side == DiceSide.Blank)
                    dice.Reroll();
            }
            if (type == "failures")
            {
                if ((dice.Side == DiceSide.Blank) || (dice.Side == DiceSide.Focus))
                    dice.Reroll();
            }
        }
    }

    public void ApplyFocus()
    {
        ChangeAll(DiceSide.Focus, DiceSide.Success);
    }

    public void ApplyEvade()
    {
        Dice addEvade = new Dice("defence", DiceSide.Success);
        DiceList.Add(addEvade);
        addEvade.NoRoll();
        OrganizeDicePositions();
    }

    public void ChangeOne(DiceSide oldSide, DiceSide newSide)
    {
        ChangeDice(oldSide, newSide, true);
    }

    public void ChangeAll(DiceSide oldSide, DiceSide newSide)
    {
        ChangeDice(oldSide, newSide, false);
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
        if (numToCancel > 0)
        {
            for (int i = 0; i < numToCancel; i++)
            {
                CancelHit();
            }
        }
    }

    public void CalculateResults(DelegateDiceroll callBack)
    {
        Dices.PlanWaitForResults(this, callBack);
    }

    public void CalculateWaitedResults()
    {
        foreach (Dice dice in DiceList)
        {
            DiceSide face = dice.GetModelFace();
            dice.SetSide(face);
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
            DiceList[i].SetPosition(Dices.DiceField.position + Dices.diceResultsOffset[i]);
        }
    }

}
