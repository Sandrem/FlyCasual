using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static partial class Phases {

    public static void UpdateHelpInfo()
    {
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Game.PrefabsList.PhasesPanel.transform.Find("PhaseText").GetComponent<Text>().text = Phases.CurrentPhase.Name;
        Game.PrefabsList.PhasesPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = Phases.CurrentSubPhase.Name;

        string playerText = "PLAYER: " + Tools.PlayerToInt(Phases.CurrentSubPhase.RequiredPlayer);
        string pilotSkillText = "PILOTS WITH SKILL: " + Phases.CurrentSubPhase.RequiredPilotSkill.ToString();

        if (Phases.CurrentPhase.GetType() == typeof(MainPhases.PlanningPhase))
        {
            pilotSkillText = "";
        }

        if (Phases.CurrentSubPhase.isTemporary)
        {
            playerText = "";
            pilotSkillText = "";
        }

        Game.PrefabsList.PhasesPanel.transform.Find("PlayerNoText").GetComponent<Text>().text = playerText;
        Game.PrefabsList.PhasesPanel.transform.Find("PilotSkillText").GetComponent<Text>().text = pilotSkillText;
    }

    public static void UpdateTemporaryState(string temporaryStateName)
    {
        Game.PrefabsList.PhasesPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = temporaryStateName;
    }

    /*protected int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }*/

}
