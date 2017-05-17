using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class PhasesManager {

    public void UpdateHelpInfo()
    {
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Game.PrefabList.PhasesPanel.transform.Find("PhaseText").GetComponent<Text>().text = Game.Phases.CurrentPhase.Name;
        Game.PrefabList.PhasesPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = Game.Phases.CurrentSubPhase.Name;

        string playerText = "PLAYER: " + Game.Phases.CurrentSubPhase.RequiredPlayer.ToString();
        string pilotSkillText = "PILOTS WITH SKILL: " + Game.Phases.CurrentSubPhase.RequiredPilotSkill.ToString();

        if (Game.Phases.CurrentPhase.GetType() == typeof(Phases.PlanningPhase))
        {
            pilotSkillText = "";
        }

        if (Game.Phases.CurrentSubPhase.isTemporary)
        {
            playerText = "";
            pilotSkillText = "";
        }

        Game.PrefabList.PhasesPanel.transform.Find("PlayerNoText").GetComponent<Text>().text = playerText;
        Game.PrefabList.PhasesPanel.transform.Find("PilotSkillText").GetComponent<Text>().text = pilotSkillText;
    }

    public void UpdateTemporaryState(string temporaryStateName)
    {
        Game.PrefabList.PhasesPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = temporaryStateName;
    }

    /*protected int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }*/

}
