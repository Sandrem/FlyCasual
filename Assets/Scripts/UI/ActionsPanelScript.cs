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

    public void ShowActionsPanel(bool afterMovement)
    {
        //todo: rewrite
        if (!Game.Selection.ThisShip.IsDestroyed) {
            Game.Selection.ThisShip.GenerateAvailableActionsList(afterMovement);
            ShowActionsButtons();
        }
        else
        {
            Game.PhaseManager.CurrentPhase.NextSubPhase();
        }
    }

    public void ShowFreeActionsPanel(bool afterMovement)
    {
        Game.Selection.ThisShip.GenerateAvailableFreeActionsList(afterMovement);
        ShowFreeActionsButtons();
    }

    private void ShowActionsButtons()
    {
        HideActionsButtons();

        float offset = 0;
        Vector3 defaultPosition = panelActions.transform.position + new Vector3(-95, 195, 0);
        foreach (var action in Game.Selection.ThisShip.AvailableActionsList)
        {
            GameObject newButton = Instantiate(prefabActionButton, panelActions.transform);
            newButton.name = "Button" + action.Key;
            newButton.transform.GetComponentInChildren<Text>().text = action.Key;
            newButton.GetComponent<RectTransform>().position = defaultPosition + new Vector3(0, -offset, 0);
            offset += 40;
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                action.Value.Invoke();
                Game.Selection.ThisShip.AlreadyExecutedActions.Add(action.Key);
                CloseActionsPanel();
            });
            newButton.GetComponent<Button>().interactable = true;
            newButton.SetActive(true);
        }

        if (Game.Selection.ThisShip.AvailableActionsList.Count != 0)
        {
            panelActions.SetActive(true);
        }
        else
        {
            Game.UI.ShowError("Cannot perform any actions");
            Game.PhaseManager.CurrentPhase.NextSubPhase();
        }
    }

    private void ShowFreeActionsButtons()
    {
        HideActionsButtons();

        float offset = 0;
        Vector3 defaultPosition = panelActions.transform.position + new Vector3(-95, 195, 0);
        foreach (var action in Game.Selection.ThisShip.AvailableFreeActionsList)
        {
            GameObject newButton = Instantiate(prefabActionButton, panelActions.transform);
            newButton.name = "Button" + action.Key;
            newButton.transform.GetComponentInChildren<Text>().text = action.Key;
            newButton.GetComponent<RectTransform>().position = defaultPosition + new Vector3(0, -offset, 0);
            offset += 40;
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                action.Value.Invoke();
                Game.Selection.ThisShip.AlreadyExecutedActions.Add(action.Key);
                CloseActionsPanel();
            });
            newButton.GetComponent<Button>().interactable = true;
            newButton.SetActive(true);
        }

        if (Game.Selection.ThisShip.AvailableFreeActionsList.Count != 0)
        {
            panelActions.SetActive(true);
        }
        else
        {
            Game.UI.ShowError("Cannot perform any actions");
            Game.Selection.isInTemporaryState = false;
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
            Game.Selection.isInTemporaryState = false;
        }
        else
        {
            Game.PhaseManager.CurrentPhase.NextSubPhase();
        }
    }

}
