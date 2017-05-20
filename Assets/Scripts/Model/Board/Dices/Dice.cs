using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dice
{

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
