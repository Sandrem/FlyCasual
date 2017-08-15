using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dice
{
    public bool IsSelected { get; private set; }

    public bool IsRerolled { get; private set; }
    public bool IsShowRerolledLock { get; private set; }

    public void ToggleSelected(bool isSelected)
    {
        IsSelected = isSelected;
        TurnOnDiceOverlay();
        Model.transform.Find("DiceOverlay/SelectionProjector").gameObject.SetActive(isSelected);
    }

    public void ToggleRerolledLock(bool isActive)
    {
        if (IsRerolled)
        {
            if (IsShowRerolledLock != isActive)
            {
                IsShowRerolledLock = isActive;
                TurnOnDiceOverlay();
                Model.transform.Find("DiceOverlay/LockedSprite").gameObject.SetActive(isActive);
            }
        }
    }

    private void TurnOnDiceOverlay()
    {
        Model.transform.Find("DiceOverlay").gameObject.transform.position = Model.transform.Find("Dice").position;
        Model.transform.Find("DiceOverlay").gameObject.SetActive(true);
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
