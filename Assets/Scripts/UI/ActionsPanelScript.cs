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
        //todo: rewrite
        if (!Game.Selection.ThisShip.IsDestroyed) {
            Game.Selection.ThisShip.GenerateAvailableActionsList();
            ShowActionsButtons(Game.Selection.ThisShip.GetAvailableActionsList());
        }
        else
        {
            Game.PhaseManager.CurrentSubPhase.NextSubPhase();
        }
    }

    public void ShowFreeActionsPanel()
    {
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
            Game.PhaseManager.CurrentSubPhase.NextSubPhase();
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
        if (Game.Selection.isInTemporaryState)
        {
            //if (Game.PhaseManager.TemporaryPhaseName == "Perform free action") Game.PhaseManager.EndTemporaryPhase();
        }
        else
        {
            Game.PhaseManager.CurrentSubPhase.NextSubPhase();
        }
    }

}
