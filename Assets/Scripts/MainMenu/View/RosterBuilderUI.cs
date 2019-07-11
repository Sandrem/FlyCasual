﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SquadBuilderNS;
using Players;

public class RosterBuilderUI : MonoBehaviour {

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = GameObject.Find("UI/Panels/ImportExportPanel/Content/InputField").GetComponent<InputField>().text;
        Messages.ShowInfo("Copied to clipboard");
    }

    public void PasteFromClipboard()
    {
        GameObject.Find("UI/Panels/ImportExportPanel/Content/InputField").GetComponent<InputField>().text = GUIUtility.systemCopyBuffer;
    }

    public void Import()
    {
        SquadBuilder.CreateSquadFromImportedJson(
            "",
            GameObject.Find("UI/Panels/ImportExportPanel/Content/InputField").GetComponent<InputField>().text,
            SquadBuilder.CurrentPlayer,
            delegate { MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel"); }
        );
    }

    // NEW

    public void SetCurrentPlayerFactionAndNext(string factionText)
    {
        Faction faction = (Faction) Enum.Parse(typeof(Faction), factionText);
        SquadBuilder.SetCurrentPlayerFaction(faction);

        SquadBuilder.ReturnToSquadBuilder();
    }

    public void RemoveCurrentShip()
    {
        SquadBuilder.RemoveCurrentShip();
        SquadBuilder.ReturnToSquadBuilder();
    }

    public void NextPlayer()
    {
        if (SquadBuilder.ValidateCurrentPlayersRoster())
        {
            if (!SquadBuilder.IsVsAiGame)
            {
                NextPlayerOpen();
            }
            else
            {
                MainMenu.CurrentMainMenu.ChangePanel("AiDecisionPanel");
            }
        }
    }

    public void NextPlayerOpen()
    {
        SquadBuilder.SetCurrentPlayer(PlayerNo.Player2);
        MainMenu.CurrentMainMenu.ChangePanel("SelectFactionPanel");
    }

    public void NextPlayerRandomAi()
    {
        SquadBuilder.SetCurrentPlayer(PlayerNo.Player2);
        SquadBuilder.SetRandomAiSquad(StartBattle);
    }

    public void FactionSelectionBackIsPressed()
    {
        if (SquadBuilder.CurrentPlayer == PlayerNo.Player1)
        {
            MainMenu.CurrentMainMenu.ChangePanel("GameModeDecisionPanel");
        }
        else if (SquadBuilder.CurrentPlayer == PlayerNo.Player2)
        {
            if (!SquadBuilder.IsVsAiGame)
            {
                SquadBuilder.SetCurrentPlayer(PlayerNo.Player1);
                SquadBuilder.ReturnToSquadBuilder();
            }
            else
            {
                MainMenu.CurrentMainMenu.ChangePanel("AiDecisionPanel");
            }
        }
    }

    public void AiDecisionPanelBack()
    {
        SquadBuilder.SetCurrentPlayer(PlayerNo.Player1);
        MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
    }

    public void ClearSquadList()
    {
        SquadBuilder.ClearShipsOfPlayer(SquadBuilder.CurrentPlayer);
        // TODO: Set default name for Squad
        SquadBuilder.ReturnToSquadBuilder();
    }

    public void OpenImportPanel()
    {
        SquadBuilder.OpenImportExportPanel(true);
    }

    public void OpenExportPanel()
    {
        SquadBuilder.OpenImportExportPanel(false);
    }

    public void TrySaveSquadron()
    {
        string squadName = GameObject.Find("UI/Panels/SaveSquadronPanel/Panel/Name/InputField").GetComponent<InputField>().text;
        if (squadName == "") squadName = "Unnamed squadron";

        SquadBuilder.SaveSquadron(SquadBuilder.CurrentSquadList, squadName, SquadBuilder.ReturnToSquadBuilder);
    }

    public void StartBattle()
    {
        if (SquadBuilder.ValidateCurrentPlayersRoster())
        {
            SquadBuilder.SaveAutosaveSquadConfigurations();

            if (!SquadBuilder.IsNetworkGame)
            {
                GameController.StartBattle();
            }
            else
            {
                MainMenu.CurrentMainMenu.ChangePanel("MultiplayerDecisionPanel");
            }
        }
    }

    public void LoadSquadDirectly()
    {
        MainMenu.CurrentMainMenu.ChangePanel("BrowseSavedSquadsPanel");
    }

    public void ChangeAiMode(Text textComponent)
    {
        SquadBuilder.ToggleAiType();
    }

    public void ClearUpgradesOfCurrentShip()
    {
        SquadBuilder.ClearUpgradesOfCurrentShip();
    }

    public void CopyCurrentShip()
    {
        SquadBuilder.CopyCurrentShip();
    }

    public void SetDefaultObstacles()
    {
        SquadBuilder.SetDefaultObstacles();
    }

    public void OnUpgradeFilterTextChanged(InputField input)
    {
        SquadBuilder.FilterVisibleUpgrades(input.text.ToLower());
    }

}
