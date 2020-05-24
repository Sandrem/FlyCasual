using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;

public enum DiceRollCheckType
{
    Combat,
    Check,
    Virtual
}

public partial class DiceRoll
{
    // Important
    public static DiceRoll CurrentDiceRoll { get; set; }
    public List<Die> DiceList { get; private set; } = new List<Die>();
    public int Count { get { return DiceList.Count; } }
    public bool IsEmpty { get { return DiceList.Count == 0; } }
    public DiceKind Type { get; private set; }
    public PlayerNo Owner { get; private set; }
    public DiceRollCheckType CheckType { get; private set; }
    private DelegateDiceroll CallBack { get; set; }

    // Roll
    public int CountOfInitialRoll { get; private set; }

    // Rerolls
    public List<Die> DiceRerolled { get; private set; } = new List<Die>();
    public int NotRerolled { get { return DiceList.Count(n => (n.IsRerolled == false)); } }
    public int FocusesNotRerolled { get { return DiceList.Count(n => ((n.Side == DieSide.Focus) && (n.IsRerolled == false))); } }
    public int BlanksNotRerolled { get { return DiceList.Count(n => ((n.Side == DieSide.Blank) && (n.IsRerolled == false))); } }

    // Modifications
    public int CanBeModified { get { return DiceList.Count(n => n.CannotBeModified == false); } }
    public int CannotBeModified { get { return DiceList.Count(n => n.CannotBeModified == true); } }
    public List<PlayerNo> ModifiedByPlayers { get; private set; } = new List<PlayerNo>();

    // Seclection
    public List<Die> Selected { get { return DiceList.Where(n => (n.IsSelected)).ToList(); } }
    public int DiceWereSelectedForRerollCount { get; private set; }
    public int SelectedCount { get { return DiceList.Count(n => (n.IsSelected)); } }

    // Results
    public bool HasResult(DieSide side) { return DiceList.Any(n => n.Side == side); }
    public int Successes { get { return DiceList.Count(n => ((n.Side == DieSide.Success) || (n.Side == DieSide.Crit))); } }
    public int RegularSuccesses { get { return DiceList.Count(n => (n.Side == DieSide.Success)); } }
    public int CriticalSuccesses { get { return DiceList.Count(n => (n.Side == DieSide.Crit)); } }
    public int Failures { get { return DiceList.Count(n => ((n.Side == DieSide.Blank) || (n.Side == DieSide.Focus))); } }
    public int Focuses { get { return DiceList.Count(n => (n.Side == DieSide.Focus)); } }
    public int Blanks { get { return DiceList.Count(n => (n.Side == DieSide.Blank)); } }
    public DieSide WorstResult
    {
        get
        {
            if (Blanks > 0) return DieSide.Blank;
            if (Focuses > 0) return DieSide.Focus;
            if (RegularSuccesses > 0) return DieSide.Success;
            if (CriticalSuccesses > 0) return DieSide.Crit;

            return DieSide.Unknown;
        }
    }

    // Cancel dice
    public int SuccessesCancelable { get { return DiceList.Count(n => ((n.Side == DieSide.Success) || (n.Side == DieSide.Crit)) && (n.IsUncancelable == false)); } }

    // Special
    public bool CancelCritsFirst;
    public DieSide[] ResultsArray { get { return this.DiceList.Select(n => n.Side).ToArray(); } }

    // View
    public Transform SpawningPoint { get; set; }
    public Transform FinalPositionPoint { get; set; }
    private bool ShouldSkipToSync() { return (ReplaysManager.Mode == ReplaysMode.Read) || (Network.IsNetworkGame && !Network.IsServer); }
}
