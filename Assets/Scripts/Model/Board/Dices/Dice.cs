using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dice
{
    public bool IsSelected { get; private set; }

    public void ToggleSelected(bool isSelected)
    {
        IsSelected = isSelected;
        Model.transform.Find("DiceOverlay").gameObject.transform.position = Model.transform.Find("Dice").position;
        Model.transform.Find("DiceOverlay").gameObject.SetActive(isSelected);
        Model.transform.Find("DiceOverlay/SelectionProjector").gameObject.SetActive(isSelected);
    }

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
