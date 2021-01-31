using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceStatsValues : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < 3; i++)
        {
            int overallAT = 0;
            int overallDT = 0;
            int overallAll = 0;

            string statsString = DiceStatsTracker.DiceStats[Tools.IntToPlayer(i)].ToString();
            string[] statBlocks = statsString.Split('|');
            foreach (var statBlock in statBlocks)
            {
                string[] diceBlock = statBlock.Split('-');
                if (diceBlock[0] == "AT") overallAT = int.Parse(diceBlock[1]);
                else if (diceBlock[0] == "DT") overallDT = int.Parse(diceBlock[1]);
            }
            overallAll = overallAT + overallDT;

            this.transform.Find("Row" + i + "/T").GetComponent<Text>().text = overallAll.ToString();

            foreach (var statBlock in statBlocks)
            {
                string[] diceBlock = statBlock.Split('-');

                string diceStatsText = "";
                switch (diceBlock[0])
                {
                    case "AC":
                        diceStatsText = GenerateDiceStatText(int.Parse(diceBlock[1]), 1f / 8f, overallAT);
                        break;
                    case "AE":
                    case "AB":
                        diceStatsText = GenerateDiceStatText(int.Parse(diceBlock[1]), 2f / 8f, overallAT);
                        break;
                    case "DE":
                        diceStatsText = GenerateDiceStatText(int.Parse(diceBlock[1]), 2f / 8f, overallDT);
                        break;
                    case "AS":
                        diceStatsText = GenerateDiceStatText(int.Parse(diceBlock[1]), 3f / 8f, overallAT);
                        break;
                    case "DS":
                    case "DB":
                        diceStatsText = GenerateDiceStatText(int.Parse(diceBlock[1]), 3f / 8f, overallDT);
                        break;
                    default:
                        diceStatsText = diceBlock[1];
                        break;
                }
                this.transform.Find("Row" + i + "/" + diceBlock[0]).GetComponent<Text>().text = diceStatsText;
            }
        }
    }

    private string GenerateDiceStatText(float realCount, float expectedChance, float overall)
    {
        float realChance = realCount / overall;
        string result = "0";
        if (overall != 0) { result = String.Format("{0} ({1:+0.00;-0.00;0.00}%)", realCount, (realChance - expectedChance) * 100); }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
