using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dice
{
    public void Cancel()
    {
        Side = DiceSide.Blank;
    }

    public void SetSide(DiceSide side)
    {
        Side = side;
    }

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

}
