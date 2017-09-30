using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static partial class Phases {

    public static void UpdateHelpInfo()
    {
        GameObject.Find("UI/PhasesPanel").transform.Find("PhaseText").GetComponent<Text>().text = CurrentPhase.Name;
        GameObject.Find("UI/PhasesPanel").transform.Find("SubPhaseText").GetComponent<Text>().text = CurrentSubPhase.Name;

        string playerText = "PLAYER: " + Tools.PlayerToInt(CurrentSubPhase.RequiredPlayer);
        string pilotSkillText = "PILOTS WITH SKILL: " + CurrentSubPhase.RequiredPilotSkill.ToString();

        if (CurrentPhase.GetType() == typeof(MainPhases.PlanningPhase))
        {
            pilotSkillText = "";
        }

        if (CurrentSubPhase.IsTemporary)
        {
            playerText = "";
            pilotSkillText = "";
        }

        GameObject.Find("UI/PhasesPanel").transform.Find("PlayerNoText").GetComponent<Text>().text = playerText;
        GameObject.Find("UI/PhasesPanel").transform.Find("PilotSkillText").GetComponent<Text>().text = pilotSkillText;
    }

    public static void UpdateTemporaryState(string temporaryStateName)
    {
        GameObject.Find("UI/PhasesPanel").transform.Find("SubPhaseText").GetComponent<Text>().text = temporaryStateName;
    }

}
