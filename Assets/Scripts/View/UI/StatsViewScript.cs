using Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDiceStats
{
    public PlayerNo PlayerNo { get; private set; }
    public List<DiceStatsEntry> DiceStats { get; private set; }
    private int TotalDice;
    private Text TotalDiceText;

    private static Dictionary<DieSide, float> AttackDiceChances = new Dictionary<DieSide, float>()
    {
        {DieSide.Crit, 1f/8f },
        {DieSide.Success, 3f/8f },
        {DieSide.Focus, 2f/8f },
        {DieSide.Blank, 2f/8f }
    };

    private static Dictionary<DieSide, float> DefenseDiceChances = new Dictionary<DieSide, float>()
    {
        {DieSide.Success, 3f/8f },
        {DieSide.Focus, 2f/8f },
        {DieSide.Blank, 3f/8f }
    };

    private static Dictionary<DiceKind, Dictionary<DieSide, float>> DiceChances = new Dictionary<DiceKind, Dictionary<DieSide, float>>()
    {
        {DiceKind.Attack, AttackDiceChances },
        {DiceKind.Defence, DefenseDiceChances }
    };

    public PlayerDiceStats(PlayerNo playerNo, StatsViewScript script, string initializationString = "")
    {
        PlayerNo = playerNo;

        DiceStats = new List<DiceStatsEntry>()
        {
            new DiceStatsEntry(DiceKind.Attack, DieSide.Unknown, this, script),
            new DiceStatsEntry(DiceKind.Attack, DieSide.Crit, this, script),
            new DiceStatsEntry(DiceKind.Attack, DieSide.Success, this, script),
            new DiceStatsEntry(DiceKind.Attack, DieSide.Focus, this, script),
            new DiceStatsEntry(DiceKind.Attack, DieSide.Blank, this, script),
            new DiceStatsEntry(DiceKind.Defence, DieSide.Unknown, this, script),
            new DiceStatsEntry(DiceKind.Defence, DieSide.Success, this, script),
            new DiceStatsEntry(DiceKind.Defence, DieSide.Focus, this, script),
            new DiceStatsEntry(DiceKind.Defence, DieSide.Blank, this, script),
        };

        if (script != null)
        {
            TotalDiceText = script.transform.Find("General/TextDiceP" + Tools.PlayerToInt(PlayerNo)).GetComponent<Text>();
        }

        if (initializationString != "")
        {
            InitializeDiceStats(initializationString);
        }
    }

    private void InitializeDiceStats(string initializationString)
    {
        string[] statBlocks = initializationString.Split('|');

        foreach (var statBlock in statBlocks)
        {
            string[] valueBlock = statBlock.Split('-');
            int value = int.Parse(valueBlock[1]);

            switch (valueBlock[0])
            {
                case "AT":
                    DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Unknown).Count = value;
                    break;
                case "AC":
                    DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Crit).Count = value;
                    break;
                case "AS":
                    DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Success).Count = value;
                    break;
                case "AE":
                    DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Focus).Count = value;
                    break;
                case "AB":
                    DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Blank).Count = value;
                    break;
                case "DT":
                    DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Unknown).Count = value;
                    break;
                case "DS":
                    DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Success).Count = value;
                    break;
                case "DE":
                    DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Focus).Count = value;
                    break;
                case "DB":
                    DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Blank).Count = value;
                    break;
                default:
                    break;
            }
        }
    }

    public override string ToString()
    {
        string result = "AT-" + DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Unknown).Count +
            "|AC-" + DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Crit).Count +
            "|AS-" + DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Success).Count +
            "|AE-" + DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Focus).Count +
            "|AB-" + DiceStats.First(n => n.DiceKind == DiceKind.Attack && n.DieSide == DieSide.Blank).Count +
            "|DT-" + DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Unknown).Count +
            "|DS-" + DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Success).Count +
            "|DE-" + DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Focus).Count +
            "|DB-" + DiceStats.First(n => n.DiceKind == DiceKind.Defence && n.DieSide == DieSide.Blank).Count;

        return result;
    }

    public void ProcessRolledDice(DieRollEventArg args)
    {
        DiceStatsEntry entry = DiceStats.Find(n => n.DiceKind == args.DiceKind && n.DieSide == DieSide.Unknown);
        entry.Count++;
        entry.Text.text = entry.Count.ToString();

        TotalDice++;
        TotalDiceText.text = DiceStats.Where(n => n.DieSide == DieSide.Unknown).Sum(n => n.Count).ToString();
    }

    private void UpdateStats(DiceKind diceKind)
    {
        foreach (DiceStatsEntry entry in DiceStats.Where(n => n.DiceKind == diceKind && n.DieSide != DieSide.Unknown))
        {
            int rolled = DiceStats.Find(n => n.DiceKind == diceKind && n.DieSide == DieSide.Unknown).Count;
            float part = (float)entry.Count / (float)rolled;
            float difference = part - DiceChances[entry.DiceKind][entry.DieSide];
            entry.Text.text = String.Format("{0} ({1:+0.00;-0.00;0.00}%)", entry.Count, difference * 100);
        }
    }

    public void ProcessDiceResult(DieResultEventArg args)
    {
        DiceStatsEntry entry = DiceStats.Find(n => n.DiceKind == args.DiceKind && n.DieSide == args.DieSide);
        entry.Count++;
        //entry.Text.text = entry.Count.ToString();

        UpdateStats(args.DiceKind);
    }

    public void Update(PlayerDiceStats playerDiceStats)
    {
        foreach (var item in playerDiceStats.DiceStats)
        {
            DiceStats.First(n => n.DiceKind == item.DiceKind && n.DieSide == item.DieSide).Count += item.Count;
        }
    }
}

public class DiceStatsEntry
{
    public DiceKind DiceKind { get; private set; }
    public DieSide DieSide { get; private set; }
    public int Count;
    public Text Text { get; private set; }
    public PlayerDiceStats PlayerStats { get; private set; }

    public DiceStatsEntry(DiceKind diceKind, DieSide dieSide, PlayerDiceStats playerStats, StatsViewScript script)
    {
        DiceKind = diceKind;
        DieSide = dieSide;
        PlayerStats = playerStats;

        if (script != null)
        {
            Text = script.transform.Find(DiceKind.ToString()).Find("P" + Tools.PlayerToInt(PlayerStats.PlayerNo)).Find(DieSide.ToString()).GetComponent<Text>();
        }
    }
}

public class StatsViewScript : MonoBehaviour
{
    public static StatsViewScript Instance;

    private Dictionary<PlayerNo, PlayerDiceStats> PlayerStats;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats = new Dictionary<PlayerNo, PlayerDiceStats>()
        {
            { PlayerNo.Player1, new PlayerDiceStats(PlayerNo.Player1, this) },
            { PlayerNo.Player2, new PlayerDiceStats(PlayerNo.Player2, this) }
        };

        DiceManager.OnDiceRolled += ProcessRolledDice;
        DiceManager.OnDiceResult += ProcessDiceResult;

        Instance = this;
    }

    private void ProcessDiceResult(object sender, EventArgs e)
    {
        if (this == null) return;

        DieResultEventArg args = e as DieResultEventArg;
        PlayerStats[args.PlayerNo].ProcessDiceResult(args);
    }

    private void ProcessRolledDice(object sender, EventArgs e)
    {
        if (this == null) return;

        DieRollEventArg args = e as DieRollEventArg;
        PlayerStats[args.PlayerNo].ProcessRolledDice(args);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerDiceStats GetStats(PlayerNo playerNo)
    {
        return PlayerStats[playerNo];
    }
}
