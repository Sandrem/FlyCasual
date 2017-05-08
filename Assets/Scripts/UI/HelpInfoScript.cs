using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpInfoScript : MonoBehaviour {

    public GameObject HelpPanel;

    void Start()
    {

    }

    public void UpdateHelpInfo(Phases currentPhase, SubPhases currentSubPhase, int currentPlayer, int currentPilotSkill)
    {
        string currentPhaseText = "";

        switch (currentPhase)
        {
            case Phases.Setup:
                currentPhaseText = "Setup Phase";
                break;
            case Phases.Planning:
                currentPhaseText = "Planning Phase";
                break;
            case Phases.Activation:
                currentPhaseText = "Activation Phase";
                break;
            case Phases.Combat:
                currentPhaseText = "Combat Phase";
                break;
            case Phases.End:
                currentPhaseText = "End Phase";
                break;
        }

        string currentSubPhaseText = "";

        switch (currentSubPhase)
        {
            case SubPhases.Setup:
                currentSubPhaseText = "Place ships into highlighted zone";
                break;
            case SubPhases.AssignManeuvers:
                currentSubPhaseText = "Assign maneuver to ship";
                break;
            case SubPhases.PerformManeuver:
                currentSubPhaseText = "Perform ship's assigned maneuver";
                break;
            case SubPhases.PerformAction:
                currentSubPhaseText = "Perform ship's action";
                break;
            case SubPhases.PerformAttack:
                currentSubPhaseText = "Perform ship's attack";
                break;
        }

        HelpPanel.transform.Find("PhaseText").GetComponent<Text>().text = currentPhaseText;
        HelpPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = currentSubPhaseText;
        HelpPanel.transform.Find("PlayerNoText").GetComponent<Text>().text = "PLAYER: " + currentPlayer;
        HelpPanel.transform.Find("PilotSkillText").GetComponent<Text>().text = (currentPhase == Phases.Planning) ? "" : "PILOTS WITH SKILL: " + currentPilotSkill.ToString();
    }

    public void UpdateTemporaryState(string temporaryStateName)
    {
        HelpPanel.transform.Find("SubPhaseText").GetComponent<Text>().text = temporaryStateName;
    }

}
