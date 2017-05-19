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

}
