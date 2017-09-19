using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Todo: Move to different scripts by menu names

public class UI : MonoBehaviour {

    private static float lastLogTextPosition = -5;
    private static float lastLogTextStep = -20;

    private int minimapSize = 256;

    public static bool ShowShipIds;

    public void Update()
    {
        UpdateShipIds();
    }

    private void UpdateShipIds()
    {
        ShowShipIds = Input.GetKey(KeyCode.LeftAlt);
    }

    //Move to context menu
    public static void CallContextMenu(Ship.GenericShip ship)
    {
        ShowContextMenu(ship, Input.mousePosition + new Vector3(0f, 0f, 0f));
    }

    private static void ShowContextMenu(Ship.GenericShip ship, Vector3 position)
    {
        HideDirectionMenu();
        HideContextMenuButtons();
        if (Phases.CurrentSubPhase.CountActiveButtons(ship) > 0)
        {
            GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(true);
            position = FixMenuPosition(GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject, position);
            GameObject.Find("UI").transform.Find("ContextMenuPanel").position = position;
        }
        else
        {
            GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        }

    }

    private static void HideContextMenuButtons()
    {
        foreach (Transform button in GameObject.Find("UI").transform.Find("ContextMenuPanel"))
        {
            button.gameObject.SetActive(false);
        }
    }

    public static void HideContextMenu()
    {
        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
    }

    public void ShowDirectionMenu()
    {
        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        CustomizeDirectionsMenu();
        GameObject.Find("UI").transform.Find("DirectionsPanel").position = FixMenuPosition(GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject, GameObject.Find("UI").transform.Find("ContextMenuPanel").position);
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(true);
    }

    private void ClearAvailableManeuvers()
    {
        for (int i = 0; i < 7; i++)
        {
            GameObject line = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Directions").Find("Speed" + i).gameObject;
            foreach (Transform button in line.transform)
            {
                button.gameObject.SetActive(false);
            }

            GameObject number = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Numbers").Find("Speed" + i).Find("Number").gameObject;
            number.SetActive(false);
        }
    }

    private void CustomizeDirectionsMenu()
    {
        ClearAvailableManeuvers();
        RestoreDirectionsMenu();

        List<int> linesExist = new List<int>();

        foreach (KeyValuePair<string, Movement.ManeuverColor> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];

            GameObject button = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Directions").Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
            if (maneuverData.Value != Movement.ManeuverColor.None)
            {
                if (!linesExist.Contains(int.Parse(maneuverSpeed))) linesExist.Add(int.Parse(maneuverSpeed));

                SetManeuverColor(button, maneuverData);
                button.SetActive(true);

                GameObject number = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                number.SetActive(true);
            }
        }

        HideExtraLines(linesExist);
        HideExtraColumns();
    }

    private void HideExtraColumns()
    {
        List<string> columns = new List<string>() { ".L.E", ".L.R", ".F.R", ".R.R", ".R.E"};
        GameObject directionsPanel = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Directions").gameObject;
        int columnCounter = 0;

        foreach (var column in columns)
        {
            bool columnExists = false;
            for (int i = 1; i < 6; i++)
            {
                Transform directionIcon = directionsPanel.transform.Find("Speed" + i).Find(i + column);
                if (directionIcon != null && directionIcon.gameObject.activeSelf)
                {
                    columnExists = true;
                    //break;
                }
            }
            if (columnExists)
            {
                for (int i = 1; i < 6; i++)
                {
                    Transform directionIcon = directionsPanel.transform.Find("Speed" + i).Find(i + column);
                    if (directionIcon != null && directionIcon.gameObject.activeSelf)
                    {
                        directionIcon.localPosition = new Vector2(205 + columnCounter * 40, directionIcon.localPosition.y);
                    }
                }
                columnCounter++;
            }
            else
            {
                directionsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(directionsPanel.GetComponent<RectTransform>().sizeDelta.x - 40, directionsPanel.GetComponent<RectTransform>().sizeDelta.y);
            }
        }
    }

    private void RestoreDirectionsMenu()
    {
        GameObject numbersLinePanel = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Numbers").gameObject;
        GameObject directionsLinePanel = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Directions").gameObject;
        for (int i = 0; i < 7; i++)
        {
            numbersLinePanel.transform.Find("Speed" + i).gameObject.SetActive(true);
            directionsLinePanel.transform.Find("Speed" + i).gameObject.SetActive(true);
        }
        numbersLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(numbersLinePanel.GetComponent<RectTransform>().sizeDelta.x, 290);
        directionsLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(410, 290);

        for (int i = -1; i < 7; i++)
        {
            int iFixed = (i != -1) ? i : 6;

            GameObject numbersPanel = numbersLinePanel.transform.Find("Speed" + iFixed).gameObject;
            numbersPanel.transform.localPosition = new Vector2(numbersPanel.transform.localPosition.x, -200 + i*40);

            GameObject directionsPanel = directionsLinePanel.transform.Find("Speed" + iFixed).gameObject;
            directionsPanel.transform.localPosition = new Vector2(directionsPanel.transform.localPosition.x, -205 + i * 40);
        }
    }

    private void HideExtraLines(List<int> linesExist)
    {
        for (int i = -1; i < 6; i++)
        {
            Debug.Log(i + ": " + linesExist.Contains(i));
            if (!linesExist.Contains(i))
            {
                HideLine(i);
            }
        }
    }

    private void HideLine(int row)
    {
        int rowFixed = (row != -1) ? row : 6;

        GameObject numbersLinePanel = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Numbers").gameObject;
        numbersLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);
        numbersLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(numbersLinePanel.GetComponent<RectTransform>().sizeDelta.x, numbersLinePanel.GetComponent<RectTransform>().sizeDelta.y - 40);

        GameObject directionsLinePanel = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Directions").gameObject;
        directionsLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);
        directionsLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(directionsLinePanel.GetComponent<RectTransform>().sizeDelta.x, directionsLinePanel.GetComponent<RectTransform>().sizeDelta.y - 40);

        if (row > 3)
        {
            for (int i = -1; i < rowFixed; i++)
            {
                int iFixed = (i != -1) ? i : 6;

                GameObject numbersPanel = numbersLinePanel.transform.Find("Speed" + iFixed).gameObject;
                numbersPanel.transform.localPosition = new Vector2(numbersPanel.transform.localPosition.x, numbersPanel.transform.localPosition.y + 40);

                GameObject directionsPanel = directionsLinePanel.transform.Find("Speed" + iFixed).gameObject;
                directionsPanel.transform.localPosition = new Vector2(directionsPanel.transform.localPosition.x, directionsPanel.transform.localPosition.y + 40);
            }
        }
    }

    private void SetManeuverColor(GameObject button, KeyValuePair<string, Movement.ManeuverColor> maneuverData)
    {
        Movement.MovementStruct movement = new Movement.MovementStruct(maneuverData.Key);
        Color maneuverColor = Color.yellow;

        if (maneuverData.Value == Movement.ManeuverColor.Green) maneuverColor = Color.green;
        if (maneuverData.Value == Movement.ManeuverColor.White) maneuverColor = Color.white;
        if (maneuverData.Value == Movement.ManeuverColor.Red) maneuverColor = Color.red;
        button.GetComponentInChildren<Text>().color = maneuverColor;
    }

    public static void HideDirectionMenu()
    {
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(false);
    }

    public static void HideTemporaryMenus()
    {
        HideContextMenu();
        HideDirectionMenu();
    }

    private static Vector3 FixMenuPosition(GameObject menuPanel, Vector3 position) {
        if (position.x + menuPanel.GetComponent<RectTransform>().rect.width > Screen.width) {
            position = new Vector3(Screen.width - menuPanel.GetComponent<RectTransform>().rect.width - 5, position.y, 0);
        }
        if (position.y - menuPanel.GetComponent<RectTransform>().rect.height < 0)
        {
            position = new Vector3(position.x, menuPanel.GetComponent<RectTransform>().rect.height + 5, 0);
        }
        return position;
    }

    public static void ShowGameResults(string results)
    {
        GameObject gameResultsPanel = GameObject.Find("UI").transform.Find("GameResultsPanel").gameObject;
        gameResultsPanel.transform.Find("Panel").transform.Find("Congratulations").GetComponent<Text>().text = results;
        gameResultsPanel.SetActive(true);
    }

    public static void ToggleInGameMenu()
    {
        GameObject gameResultsPanel = GameObject.Find("UI").transform.Find("GameResultsPanel").gameObject;
        gameResultsPanel.SetActive(!gameResultsPanel.activeSelf);
    }

    public void CallChangeMiniMapSize()
    {
        switch (minimapSize)
        {
            case 128: minimapSize = 256;
                break;
            case 256: minimapSize = 512;
                break;
            case 512: minimapSize = 128;
                break;
            default:
                break;
        }
        GameObject.Find("UI").transform.Find("MiniMapHolder").GetComponent<RectTransform>().sizeDelta = new Vector2(minimapSize, minimapSize);
    }

    public void ToggleMinimap()
    {
        GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.SetActive(!GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.activeSelf);
    }

    public void ToggleGameLog()
    {
        GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.SetActive(!GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.activeSelf);
    }

    public static void AddTestLogEntry(string text)
    {
        GameObject area = GameObject.Find("UI").transform.Find("GameLogHolder").Find("Scroll").Find("Viewport").Find("Content").gameObject;
        GameObject logText = (GameObject)Resources.Load("Prefabs/LogText", typeof(GameObject));
        GameObject newLogEntry = Instantiate(logText, area.transform);
        newLogEntry.transform.localPosition = new Vector3(5, lastLogTextPosition, 0);
        lastLogTextPosition += lastLogTextStep;
        if (area.GetComponent<RectTransform>().sizeDelta.y < Mathf.Abs(lastLogTextPosition)) area.GetComponent<RectTransform>().sizeDelta = new Vector2(area.GetComponent<RectTransform>().sizeDelta.x, Mathf.Abs(lastLogTextPosition));
        GameObject.Find("UI").transform.Find("GameLogHolder").Find ("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        newLogEntry.GetComponent<Text>().text = text;
    }

    public void ShowDecisionsPanel()
    {
        //start subphase

        GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(true);
    }

    public void HideDecisionsPanel()
    {
        GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(false);
        //end subphase
    }

    public void ClickNextPhase()
    {
        HideNextButton();
        Roster.AllShipsHighlightOff();

        Phases.CurrentSubPhase.NextButton();
    }

    public void ClickSkipPhase()
    {
        HideNextButton();
        Roster.AllShipsHighlightOff();

        Phases.CurrentSubPhase.SkipButton();
    }

    public void ClickDeclareTarget()
    {
        Combat.DeclareTarget();
    }

    public static void ShowNextButton()
    {
        GameObject.Find("UI").transform.Find("NextPanel").gameObject.SetActive(true);
        GameObject.Find("UI/NextPanel").transform.Find("NextButton").GetComponent<Animator>().enabled = false;
    }

    public static void HideNextButton()
    {
        GameObject.Find("UI").transform.Find("NextPanel").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Animator>().enabled = false;

        ColorBlock colors = GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Button>().colors;
        colors.normalColor = new Color32(0, 0, 0, 200);
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Button>().colors = colors;
    }

    public static void ShowSkipButton()
    {
        GameObject.Find("UI").transform.Find("SkipPanel").gameObject.SetActive(true);
    }

    public static void HideSkipButton()
    {
        GameObject.Find("UI").transform.Find("SkipPanel").gameObject.SetActive(false);
    }

    public static void HighlightNextButton()
    {
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Animator>().enabled = true;
    }

    public static void CallHideTooltip()
    {
        Tooltips.EndTooltip();
    }

    public void HideInformCritPanel()
    {
        InformCrit.HidePanel();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
