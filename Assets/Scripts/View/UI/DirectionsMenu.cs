using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class DirectionsMenu
{
    public static void Show(Func<string, bool> filter = null)
    {
        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        CustomizeDirectionsMenu(filter);
        GameObject.Find("UI").transform.Find("DirectionsPanel").position = FixMenuPosition(GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject, GameObject.Find("UI").transform.Find("ContextMenuPanel").position);
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(true);
    }

    private static void ClearAvailableManeuvers()
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

    private static void CustomizeDirectionsMenu(Func<string, bool> filter = null)
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
                if (filter == null || filter(maneuverData.Key))
                {
                    if (!linesExist.Contains(int.Parse(maneuverSpeed))) linesExist.Add(int.Parse(maneuverSpeed));

                    SetManeuverColor(button, maneuverData);
                    button.SetActive(true);

                    GameObject number = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                    number.SetActive(true);
                }
            }
        }

        HideExtraLines(linesExist);
        HideExtraColumns();
    }

    private static void HideExtraColumns()
    {
        List<string> columns = new List<string>() { ".L.E", ".L.R", ".F.R", ".R.R", ".R.E" };
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

    private static void RestoreDirectionsMenu()
    {
        GameObject directionsMenuPanel = GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject;
        GameObject numbersLinePanel = directionsMenuPanel.transform.Find("Numbers").gameObject;
        GameObject directionsLinePanel = directionsMenuPanel.transform.Find("Directions").gameObject;
        for (int i = 0; i < 7; i++)
        {
            numbersLinePanel.transform.Find("Speed" + i).gameObject.SetActive(true);
            directionsLinePanel.transform.Find("Speed" + i).gameObject.SetActive(true);
        }
        directionsMenuPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(450, 290);
        numbersLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(numbersLinePanel.GetComponent<RectTransform>().sizeDelta.x, 290);
        directionsLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(410, 290);

        for (int i = -1; i < 7; i++)
        {
            int iFixed = (i != -1) ? i : 6;

            GameObject numbersPanel = numbersLinePanel.transform.Find("Speed" + iFixed).gameObject;
            numbersPanel.transform.localPosition = new Vector2(numbersPanel.transform.localPosition.x, -200 + i * 40);

            GameObject directionsPanel = directionsLinePanel.transform.Find("Speed" + iFixed).gameObject;
            directionsPanel.transform.localPosition = new Vector2(directionsPanel.transform.localPosition.x, -205 + i * 40);
        }
    }

    private static void HideExtraLines(List<int> linesExist)
    {
        for (int i = -1; i < 6; i++)
        {
            if (!linesExist.Contains(i))
            {
                HideLine(i);
            }
        }
    }

    private static void HideLine(int row)
    {
        int rowFixed = (row != -1) ? row : 6;

        GameObject directionsMenuPanel = GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject;
        directionsMenuPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(directionsMenuPanel.GetComponent<RectTransform>().sizeDelta.x, directionsMenuPanel.GetComponent<RectTransform>().sizeDelta.y - 40);

        GameObject numbersLinePanel = directionsMenuPanel.transform.Find("Numbers").gameObject;
        numbersLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);
        numbersLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(numbersLinePanel.GetComponent<RectTransform>().sizeDelta.x, numbersLinePanel.GetComponent<RectTransform>().sizeDelta.y - 40);

        GameObject directionsLinePanel = directionsMenuPanel.transform.Find("Directions").gameObject;
        directionsLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);
        directionsLinePanel.GetComponent<RectTransform>().sizeDelta = new Vector3(directionsLinePanel.GetComponent<RectTransform>().sizeDelta.x, directionsLinePanel.GetComponent<RectTransform>().sizeDelta.y - 40);

        if (row > -1)
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

    private static void SetManeuverColor(GameObject button, KeyValuePair<string, Movement.ManeuverColor> maneuverData)
    {
        Movement.MovementStruct movement = new Movement.MovementStruct(maneuverData.Key);
        Color maneuverColor = Color.yellow;

        if (maneuverData.Value == Movement.ManeuverColor.Green) maneuverColor = Color.green;
        if (maneuverData.Value == Movement.ManeuverColor.White) maneuverColor = Color.white;
        if (maneuverData.Value == Movement.ManeuverColor.Red) maneuverColor = Color.red;
        button.GetComponentInChildren<Text>().color = maneuverColor;
    }

    private static Vector3 FixMenuPosition(GameObject menuPanel, Vector3 position)
    {
        if (position.x + menuPanel.GetComponent<RectTransform>().rect.width > Screen.width)
        {
            position = new Vector3(Screen.width - menuPanel.GetComponent<RectTransform>().rect.width - 5, position.y, 0);
        }
        if (position.y - menuPanel.GetComponent<RectTransform>().rect.height < 0)
        {
            position = new Vector3(position.x, menuPanel.GetComponent<RectTransform>().rect.height + 5, 0);
        }
        return position;
    }

    public static void Hide()
    {
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(false);
    }
}