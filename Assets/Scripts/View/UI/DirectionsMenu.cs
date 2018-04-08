using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DirectionsMenu
{
    public static bool IsForcedToShowRedManeuvers;

    public static bool IsVisible
    {
        get { return DirectionsWindow != null && DirectionsWindow.activeSelf; }
    }

    public static Action<string> Callback;

    private static Func<string, bool> currentFilter;
    private static GameObject DirectionsWindow;

    public static void Show(Action<string> callback, Func<string, bool> filter = null)
    {
        DeleteOldDirectionsMenu();

        Callback = callback;
        currentFilter = filter;

        IsForcedToShowRedManeuvers = (Input.GetKey(KeyCode.LeftControl));

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/DirectionsWindow", typeof(GameObject));
        DirectionsWindow = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/DirectionsPanel").transform);

        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        CustomizeDirectionsMenu(filter);
        DirectionsWindow.transform.position = FixMenuPosition(
            DirectionsWindow.transform.gameObject,
            Input.mousePosition
        );
    }

    public static void ShowForAll(Action<string> callback, Func<string, bool> filter = null)
    {
        DeleteOldDirectionsMenu();

        Callback = callback;
        currentFilter = filter;

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/DirectionsWindow", typeof(GameObject));
        DirectionsWindow = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/DirectionsPanel").transform);

        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        CustomizeDirectionsMenuAll(filter);
        DirectionsWindow.transform.position = FixMenuPosition(
            DirectionsWindow.transform.gameObject,
            Input.mousePosition
        );
    }

    private static void DeleteOldDirectionsMenu()
    {
        foreach (Transform transform in GameObject.Find("UI/DirectionsPanel").transform)
        {
            GameObject.Destroy(transform.gameObject);
        }
    }

    private static void ShowUpdated()
    {
        CustomizeDirectionsMenu(currentFilter);
        DirectionsWindow.transform.position = FixMenuPosition(
            DirectionsWindow.transform.gameObject,
            DirectionsWindow.transform.position
        );
    }

    private static void CustomizeDirectionsMenu(Func<string, bool> filter = null)
    {
        List<int> linesExist = new List<int>();

        foreach (KeyValuePair<string, Movement.ManeuverColor> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];

            GameObject button = DirectionsWindow.transform.Find("Directions").Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
            if (maneuverData.Value != Movement.ManeuverColor.None)
            {
                if (filter == null || filter(maneuverData.Key))
                {
                    if (!linesExist.Contains(int.Parse(maneuverSpeed))) linesExist.Add(int.Parse(maneuverSpeed));

                    SetManeuverColor(button, maneuverData);
                    button.SetActive(true);
                    button.GetComponent<Button>().onClick.AddListener(UI.AssignManeuverButtonPressed);

                    GameObject number = DirectionsWindow.transform.Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                    number.SetActive(true);
                }
            }
        }

        HideExtraElements(linesExist);
    }

    private static void CustomizeDirectionsMenuAll(Func<string, bool> filter = null)
    {
        List<int> linesExist = new List<int>();

        foreach (string maneuverCode in Movement.GenericMovement.GetAllManeuvers())
        {
            string[] parameters = maneuverCode.Split('.');
            string maneuverSpeed = parameters[0];

            GameObject button = DirectionsWindow.transform.Find("Directions").Find("Speed" + maneuverSpeed).Find(maneuverCode).gameObject;

            if (filter == null || filter(maneuverCode))
            {
                if (!linesExist.Contains(int.Parse(maneuverSpeed))) linesExist.Add(int.Parse(maneuverSpeed));

                SetManeuverColor(button, new KeyValuePair<string, Movement.ManeuverColor>(maneuverCode, Movement.ManeuverColor.White));
                button.SetActive(true);
                button.GetComponent<Button>().onClick.AddListener(UI.AssignManeuverButtonPressed);

                GameObject number = DirectionsWindow.transform.Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                number.SetActive(true);
            }

        }

        HideExtraElements(linesExist);
    }

    private static void HideExtraElements(List<int> linesExist)
    {
        float freeSpace = 40;

        // COLUMNS

        List<string> columns = new List<string>() { ".L.E", ".L.R", ".F.R", ".R.R", ".R.E" };
        GameObject directionsPanel = DirectionsWindow.transform.Find("Directions").gameObject;
        int columnCounter = 0;

        int missingColumnsCounter = 0;

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
                missingColumnsCounter++;
            }
        }

        // LINES

        for (int i = -1; i < 6; i++)
        {
            if (!linesExist.Contains(i))
            {
                int rowFixed = (i != -1) ? i : 6;

                GameObject numbersLinePanel = DirectionsWindow.transform.Find("Numbers").gameObject;
                numbersLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);

                GameObject directionsLinePanel = DirectionsWindow.transform.Find("Directions").gameObject;
                directionsLinePanel.transform.Find("Speed" + rowFixed).gameObject.SetActive(false);
            }
        }

        DirectionsWindow.GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.GetComponent<RectTransform>().sizeDelta.x, linesExist.Count * 40);
        DirectionsWindow.transform.Find("Numbers").GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.transform.Find("Numbers").GetComponent<RectTransform>().sizeDelta.x, linesExist.Count * 40);
        DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta.x - missingColumnsCounter * freeSpace, linesExist.Count * 40);

        DirectionsWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta.x + 70, DirectionsWindow.GetComponent<RectTransform>().sizeDelta.y);

        float offset = 5;
        foreach (Transform transform in DirectionsWindow.transform.Find("Numbers"))
        {
            if (transform.gameObject.activeSelf)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, offset);
                offset -= freeSpace;
            }
        }

        offset = 0;
        foreach (Transform transform in DirectionsWindow.transform.Find("Directions"))
        {
            if (transform.gameObject.activeSelf)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, offset);
                offset -= freeSpace;
            }
        }
    }

    private static void SetManeuverColor(GameObject button, KeyValuePair<string, Movement.ManeuverColor> maneuverData)
    {
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
        GameObject.Destroy(DirectionsWindow);
    }

    public static void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ForceShowRedManeuvers = true;
            if (IsVisible && !SwarmManager.IsActive) ShowUpdated();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ForceShowRedManeuvers = false;
            if (IsVisible && !SwarmManager.IsActive) ShowUpdated();
        }*/
    }
}