using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SquadBuilderNS;
using Players;

public class RosterBuilderUI : MonoBehaviour {

    /*public void CopyToClipboard()
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
    }*/

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
        if (SquadBuilder.ValidateCurrentPlayersRoster())
        {
            if (SquadBuilder.IsNetworkGame)
            {
                MainMenu.CurrentMainMenu.ChangePanel("MultiplayerDecisionPanel");
            }
            else
            {
                SquadBuilder.SetCurrentPlayer(PlayerNo.Player2);
                MainMenu.CurrentMainMenu.ChangePanel("SelectFactionPanel");
            }
        }
    }

    public void FactionSelectionBackIsPressed()
    {
        if (SquadBuilder.CurrentPlayer == PlayerNo.Player1)
        {
            MainMenu.CurrentMainMenu.ChangePanel("GameModeDecisionPanel");
        }
        else if (SquadBuilder.CurrentPlayer == PlayerNo.Player2)
        {
            SquadBuilder.SetCurrentPlayer(PlayerNo.Player1);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }
    }

    public void ClearSquadList()
    {
        SquadBuilder.ClearShipsOfCurrentPlayer();
        // TODO: Set default name for Squad
        MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
    }

}
