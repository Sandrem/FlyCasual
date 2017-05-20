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

    public string Type
    {
        get;
        private set;
    }

    public int Number { get; private set; }

}
