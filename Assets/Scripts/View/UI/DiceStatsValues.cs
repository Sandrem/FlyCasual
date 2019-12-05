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
            string statsString = DiceStatsTracker.DiceStats[Tools.IntToPlayer(i)].ToString();
            string[] statBlocks = statsString.Split('|');
            foreach (var statBlock in statBlocks)
            {
                string[] diceBlock = statBlock.Split('-');
                this.transform.Find("Row" + i + "/" + diceBlock[0]).GetComponent<Text>().text = diceBlock[1];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
