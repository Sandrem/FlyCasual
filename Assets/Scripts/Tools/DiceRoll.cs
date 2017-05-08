using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll
{

    private DiceManagementScript DiceManager;

    public List<Dice> Dices
    {
        get;
        private set;
    }

    public string Type
    {
        get;
        private set;
    }

    public int Number { get; private set; }

    public DiceRoll(string type, int number)
    {
        DiceManager = GameObject.Find("GameManager").GetComponent<DiceManagementScript>();
        Type = type;
        Number = number;
        Dices = new List<Dice>();
        for (int i = 0; i < number; i++)
        {
            Dices.Add(new Dice(type));
        }
    }

    public int Successes
    {
        get
        {
            int result = 0;
            foreach (Dice dice in Dices)
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

    public void Roll()
    {
        foreach (Dice dice in Dices)
        {
            dice.Roll();
        }
    }

    //Change to enum
    public void Reroll(string type)
    {
        foreach (Dice dice in Dices)
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
        Dices.Add(addEvade);
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
        foreach (Dice dice in Dices)
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
        Dices.Add(new Dice(Type, side));
    }

    private void CancelHit()
    {
        if (!CancelType(DiceSide.Crit))
        {
            CancelType(DiceSide.Success);
        }
    }

    private bool CancelType(DiceSide type)
    {
        bool found = false;
        foreach (Dice dice in Dices)
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

    public void CalculateResults()
    {
        DiceManager.PlanWaitForResults(this);
    }

    public void CalculateWaitedResults()
    {
        foreach (Dice dice in Dices)
        {
            DiceSide face = dice.GetModelFace();
            dice.SetSide(face);
        }
    }

    public void RemoveDiceModels()
    {
        foreach (Dice dice in Dices)
        {
            dice.RemoveModel();
        }
    }

    public void OrganizeDicePositions()
    {
        for (int i = 0; i < Dices.Count; i++)
        {
            Dices[i].SetPosition(DiceManager.diceField.transform.position + DiceManager.diceResultsOffset[i]);
        }
    }

}
