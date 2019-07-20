﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Die
{    
    public bool IsSelected { get; private set; }
    public bool IsRerolled { get; set; }
    public bool IsShowRerolledLock { get; private set; }
    public bool IsUncancelable { get; set; }

    private bool cannotBeModified { get; set; }
    public bool CannotBeModified
    {
        get
        {
            return cannotBeModified;
        }
        set
        {
            cannotBeModified = value;
            ToggleRerolledLock(IsRerolled || value); // Show the "already rerolled lock" for now. TODO: custom icon?
        }
    }

    public bool IsSuccess
    {
        get
        {
            if (Side == DieSide.Success || Side == DieSide.Crit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


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
        Side = DieSide.Blank;
    }

    public bool SetSide(DieSide side, bool isInitial = true)
    {   
        if (isInitial)
        {
            Side = side;
            return true;
        }

        bool isAllowed = true;
        // Combat.CurrentDiceRoll.
        // Issues: we don't know what the DiceModificationType is when we get into this base level function.
        // isInitial needs to be set only on the initial roll (or when specific cards want to override the functionality to treat the roll like an initial roll)
        Selection.ActiveShip.TryDiceResultModification(
            this, Abilities.GenericAbility.DiceModificationType.Change, side, ref isAllowed);
        if (isAllowed) {
            Side = side;
            return true;
        } else {
            return false;
        }
        
    }

    public DieSide Side
    {
        get;
        private set;
    }

    private List<DieSide> Sides
    {
        get;
        set;
    }

}
