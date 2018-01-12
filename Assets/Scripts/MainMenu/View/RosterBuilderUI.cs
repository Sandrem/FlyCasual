using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SquadBuilderNS;

public class RosterBuilderUI : MonoBehaviour {

    // OLD

    public void AddShip(int playerNo)
    {
        RosterBuilder.TryAddShip(Tools.IntToPlayer(playerNo));
    }

    public void OnPlayerFactionChanged()
    {
        RosterBuilder.PlayerFactonChange();
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text;
    }

    public void PasteFromClipboard()
    {
        GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text = GUIUtility.systemCopyBuffer;
    }

    public void Import()
    {
        RosterBuilder.CreateSquadFromImportedjson(GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text, Players.PlayerNo.Player1);
    }

    // NEW

    public void SetCurrentPlayerFactionAndNext(string factionText)
    {
        Faction faction = (Faction) Enum.Parse(typeof(Faction), factionText);
        SquadBuilder.SetCurrentPlayerFaction(faction);

        MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
    }

    public void RemoveCurrentShip()
    {
        SquadBuilder.RemoveCurrentShip();
        MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
    }

    public void NextPlayer()
    {
        SquadBuilder.NextPlayer();
        MainMenu.CurrentMainMenu.ChangePanel("SelectFactionPanel");
    }

}
