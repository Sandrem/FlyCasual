using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SquadBuilderNS;
using Players;

public class RosterBuilderUI : MonoBehaviour {

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text;
        Messages.ShowInfo("Copied to clipboard");
    }

    public void PasteFromClipboard()
    {
        GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text = GUIUtility.systemCopyBuffer;
    }

    public void Import()
    {
        SquadBuilder.CreateSquadFromImportedJson(GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text, SquadBuilder.CurrentPlayer);
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
        SquadBuilder.ClearShipsOfPlayer(SquadBuilder.CurrentPlayer);
        // TODO: Set default name for Squad
        MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
    }

    public void OpenImportPanel()
    {
        SquadBuilder.OpenImportExportPanel(true);
    }

    public void OpenExportPanel()
    {
        SquadBuilder.OpenImportExportPanel(false);
    }

}
