using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public class ActionsPanelScript : MonoBehaviour {

    private GameManagerScript Game;

    public GameObject panelActions;
    public GameObject prefabActionButton;

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public void ShowActionsPanel()
    {
        ShowActionsButtons(Game.Selection.ThisShip.GetAvailableActionsList());
    }

    public void ShowFreeActionsPanel()
    {
        Game.Phases.StartFreeActionSubPhase("Free action");
        ShowActionsButtons(Game.Selection.ThisShip.GetAvailableFreeActionsList());
    }

    private void ShowActionsButtons(List<Actions.GenericAction> actionList)
    {
        HideActionsButtons();

        float offset = 0;
        Vector3 defaultPosition = panelActions.transform.position + new Vector3(-95, 195, 0);
        foreach (var action in actionList)
        {
            GameObject newButton = Instantiate(prefabActionButton, panelActions.transform);
            newButton.name = "Button" + action.Name;
            newButton.transform.GetComponentInChildren<Text>().text = action.Name;
            newButton.GetComponent<RectTransform>().position = defaultPosition + new Vector3(0, -offset, 0);
            offset += 40;
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                action.ActionTake();
                Game.Selection.ThisShip.AddAlreadyExecutedAction(action);
                CloseActionsPanel();
            });
            newButton.GetComponent<Button>().interactable = true;
            newButton.SetActive(true);
        }

        if (actionList.Count != 0)
        {
            panelActions.SetActive(true);
        }
        else
        {
            Game.UI.ShowError("Cannot perform any actions");
            Game.Phases.CurrentSubPhase.NextSubPhase();
        }
    }

    public void HideActionsButtons()
    {
        foreach (Transform button in panelActions.transform)
        {
            if (button.name.StartsWith("Button"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
        }
    }

    public void CloseActionsPanel()
    {
        panelActions.SetActive(false);
        //Rework: This needs go next only if this is single-state action
        if (!(Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SelectTargetSubPhase)) && !(Game.Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase)))
        {
            Game.Phases.Next();
        }
    }

}
