using System.Collections;
using System.Collections.Generic;
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

    public int RegularSuccesses
    {
        get
        {
            int result = 0;
            foreach (Dice dice in DiceList)
            {
                if (dice.Side == DiceSide.Success)
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

    public void RerollOne()
    {
        Dice worstDice = null;
        foreach (Dice dice in DiceList)
        {
            if (dice.Side == DiceSide.Blank)
            {
                worstDice = dice;
                break;
            }
            else if (dice.Side == DiceSide.Focus)
            {
                if ((worstDice == null) || (worstDice.Side == DiceSide.Success))
                {
                    worstDice = dice;
                }
            }
            else if (dice.Side == DiceSide.Success)
            {
                if (worstDice == null)
                {
                    worstDice = dice;
                }
            }
        }
        worstDice.Reroll();
    }

    public void ApplyFocus()
    {
        ChangeAll(DiceSide.Focus, DiceSide.Success);
    }

    public void ApplyEvade()
    {
        Dice addEvade = new Dice(DiceKind.Defence, DiceSide.Success);
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

    public void CalculateResults(DelegateDiceroll callBack)
    {
        DicesManager.PlanWaitForResults(this, callBack);
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
            DiceList[i].SetPosition(DicesManager.DiceField.position + DicesManager.DicePositions[DiceList.Count-1][i]);
        }
    }

}
