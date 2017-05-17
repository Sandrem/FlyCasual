using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dice
{

    private DiceManager DiceManager;

    public string Type
    {
        get;
        private set;
    }

    public DiceSide Side
    {
        get;
        private set;
    }

    private List<DiceSide> Sides
    {
        get;
        set;
    }


    public void Cancel()
    {
        Side = DiceSide.Blank;
    }

    public void SetSide(DiceSide side)
    {
        Side = side;
    }

}
