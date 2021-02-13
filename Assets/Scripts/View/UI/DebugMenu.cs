using GameCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Text CurrentSubPhase;
    public Toggle CurrentSubPhaseReady;
    public Text PreviousSubPhase;
    public Text CurrentTrigger;
    public Text TriggerStack;
    public Text TriggerStackCount;
    public Text CurrentCommand;
    public Text CommandStackCount;
    public Text CommandStack;
    public Text LastCommandStatus;

    public void Update()
    {
        CurrentSubPhase.text = (Phases.CurrentSubPhase != null) ? Phases.CurrentSubPhase.ToString() : "None";
        PreviousSubPhase.text = (Phases.CurrentSubPhase != null && Phases.CurrentSubPhase.PreviousSubPhase != null) ? Phases.CurrentSubPhase.PreviousSubPhase.ToString() : "None";
        CurrentSubPhaseReady.isOn = (Phases.CurrentSubPhase != null) ? Phases.CurrentSubPhase.IsReadyForCommands : false;
        CurrentTrigger.text = (Triggers.CurrentTrigger != null) ? Triggers.CurrentTrigger.Name : "None";
        TriggerStackCount.text = $"   Count: {Triggers.TriggersStack.Count}";
        ShowStackContent();
        CurrentCommand.text = (GameController.CommandsReceived.Count > 0) ? GameController.CommandsReceived.First().Type.ToString() : "None";
        CommandStackCount.text = $"   Count: {GameController.CommandsReceived.Count}";
        ShowCommandsContent();
        LastCommandStatus.text = GameController.LastMessage;
    }

    private void ShowStackContent()
    {
        List<string> stackNames = new List<string>();
        foreach (StackLevel stack in Triggers.TriggersStack)
        {
            stackNames.Add(stack.TriggerType.ToString());
        }
        stackNames.Reverse();
        string stackNamesText = String.Join("\n", stackNames);
        if (stackNamesText.Length > 0) stackNamesText.Remove(stackNamesText.Length - 1);
        TriggerStack.text = stackNamesText;
    }

    private void ShowCommandsContent()
    {
        List<string> stackNames = new List<string>();
        int maxCount = 3;
        foreach (GameCommand command in GameController.CommandsReceived.Skip(1))
        {
            stackNames.Add(command.Type.ToString());
            maxCount--;
            if (maxCount == 0) break;
        }
        string stackNamesText = String.Join("\n", stackNames);
        if (stackNamesText.Length > 0) stackNamesText.Remove(stackNamesText.Length - 1);
        CommandStack.text = stackNamesText;
    }

    public void FinishSubphase()
    {
        Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
    }

    public void ResumeSubphase()
    {
        Phases.CurrentSubPhase.Resume();
    }

    public void GoBack()
    {
        Phases.GoBack();
    }

    public void OnSubphaseReadyChanged(Toggle toggle)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = toggle.isOn;
    }

    public void FinishTrigger()
    {
        if (Triggers.CurrentTrigger != null) Triggers.FinishTrigger();
    }

    public void RemoveCommand()
    {
        if (GameController.CommandsReceived.Count > 0) GameController.CommandsReceived.RemoveAt(0);
    }
}
