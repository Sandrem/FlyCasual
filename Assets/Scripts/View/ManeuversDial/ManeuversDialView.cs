using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ManeuversDialView : MonoBehaviour
{
    public GameObject ManeuverIconPrefab;
    public GameObject ManeuverIconsRowPrefab;

    public GameObject ManeuverIconsHolder;

    public float Height;
    public float Width;

    private Dictionary<ManeuverHolder, MovementComplexity> DialToShow;
    private bool IsDisabled;

    private int MinSpeed;
    private int MaxSpeed;
    private int Rows;
    private int ColumnsSpecial;
    private bool HasSpecialLeftSideColumn;
    private bool HasSpecialCenterColumn;
    private bool HasSpecialRightSideColumn;

    private const float IconSize = 100f;
    private const float SpaceSize = 50f;

    public void Initialize(Dictionary<ManeuverHolder, MovementComplexity> dialToShow, bool isDisabled)
    {
        DialToShow = dialToShow;
        IsDisabled = isDisabled;

        ProcessDialInfo();

        for (int i = MaxSpeed; i >= MinSpeed; i--)
        {
            if (!dialToShow.Any(n => n.Key.SpeedIntSigned == i)) continue;

            GameObject newIconsRow = GameObject.Instantiate<GameObject>(ManeuverIconsRowPrefab, ManeuverIconsHolder.transform);
            newIconsRow.name = "Row_" + i;
            newIconsRow.transform.Find("SpeedNumber/Text").GetComponent<Text>().text = i.ToString();

            Transform simpleManeuversTransform = newIconsRow.transform.Find("SimpleManeuvers").transform;
            CreateSimpleIcons(i, simpleManeuversTransform);
            CreateStationaryIcon(i, simpleManeuversTransform);
            CreateReverseIcons(i, simpleManeuversTransform);

            Transform specialManeuversTransform = newIconsRow.transform.Find("SpecialManeuvers").transform;
            CreateSpecialIcons(i, specialManeuversTransform);

            SetRowWidth(newIconsRow);
        }

        UpdateRowsCount();

        SetIconsHolderHeight();
        SetFinalSize();
    }

    private void ProcessDialInfo()
    {
        HasSpecialLeftSideColumn = DialToShow.Any(n => (n.Key.Bearing == ManeuverBearing.SegnorsLoop || n.Key.Bearing == ManeuverBearing.TallonRoll) && n.Key.Direction == ManeuverDirection.Left);
        HasSpecialCenterColumn = DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.KoiogranTurn);
        HasSpecialRightSideColumn = DialToShow.Any(n => (n.Key.Bearing == ManeuverBearing.SegnorsLoop || n.Key.Bearing == ManeuverBearing.TallonRoll) && n.Key.Direction == ManeuverDirection.Right);

        ColumnsSpecial = (((HasSpecialCenterColumn) ? 1 : 0) + ((HasSpecialLeftSideColumn || HasSpecialRightSideColumn) ? 2 : 0));

        MaxSpeed = DialToShow.Max(n => n.Key.SpeedIntSigned);
        MinSpeed = DialToShow.Min(n => n.Key.SpeedIntSigned);
        Rows = MaxSpeed - MinSpeed + 1;
    }

    private void CreateSimpleIcons(int i, Transform transform)
    {
        if (i <= 0) return;

        List<string> simpleKeys = new List<string>() { ".L.T", ".L.B", ".F.S", ".R.B", ".R.T" };

        foreach (string key in simpleKeys)
        {
            string maneuverId = i + key;
            if (DialToShow.Any(n => n.Key.ToString() == maneuverId))
            {
                CreateButton(DialToShow.First(n => n.Key.ToString() == maneuverId), transform);
            }
            else
            {
                CreateEmptyButton(maneuverId, transform);
            }
        }
    }

    private void CreateStationaryIcon(int i, Transform transform)
    {
        if (i != 0) return;

        if (DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.Stationary))
        {
            CreateButton(DialToShow.First(n => n.Key.Bearing == ManeuverBearing.Stationary), transform);
        }
    }

    private void CreateReverseIcons(int i, Transform transform)
    {
        if (i >= 0) return;

        List<string> reverseKeys = new List<string>() { ".L.V", ".F.V", ".R.V" };

        foreach (string key in reverseKeys)
        {
            string maneuverId = Mathf.Abs(i) + key;
            if (DialToShow.Any(n => n.Key.ToString() == maneuverId))
            {
                CreateButton(DialToShow.First(n => n.Key.ToString() == maneuverId), transform);
            }
            else
            {
                CreateEmptyButton(maneuverId, transform);
            }
        }
    }

    private void CreateSpecialIcons(int i, Transform transform)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100 * ColumnsSpecial, 100);

        if (i <= 0) return;

        List<string> leftSpecialKeys = new List<string>() { ".L.R", ".L.E" };
        List<string> rightSpecialKeys = new List<string>() { ".R.R", ".R.E" };

        bool leftSpecialButtonIsCreated = false;
        foreach (string key in leftSpecialKeys)
        {
            string maneuverId = i + key;
            if (DialToShow.Any(n => n.Key.ToString() == maneuverId))
            {
                CreateButton(DialToShow.First(n => n.Key.ToString() == maneuverId), transform);
                leftSpecialButtonIsCreated = true;
            }
        }
        if (!leftSpecialButtonIsCreated && (HasSpecialLeftSideColumn || HasSpecialRightSideColumn))
        {
            CreateEmptyButton(i + ".L.R", transform);
        }

        if (DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.KoiogranTurn))
        {
            if (DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.KoiogranTurn && n.Key.SpeedIntSigned == i))
            {
                CreateButton(DialToShow.First(n => n.Key.Bearing == ManeuverBearing.KoiogranTurn), transform);
            }
            else
            {
                if (DialToShow.Any(n => n.Key.SpeedIntSigned == i && (n.Key.Bearing == ManeuverBearing.SegnorsLoop || n.Key.Bearing == ManeuverBearing.TallonRoll)))
                {
                    CreateEmptyButton(i + ".F.R", transform);
                }
            }
        }

        bool rightSpecialButtonIsCreated = false;
        foreach (string key in rightSpecialKeys)
        {
            string maneuverId = i + key;
            if (DialToShow.Any(n => n.Key.ToString() == maneuverId))
            {
                CreateButton(DialToShow.First(n => n.Key.ToString() == maneuverId), transform);
                rightSpecialButtonIsCreated = true;
            }
        }
        if (!rightSpecialButtonIsCreated && (HasSpecialLeftSideColumn || HasSpecialRightSideColumn))
        {
            CreateEmptyButton(i + ".R.R", transform);
        }
    }

    private void CreateEmptyButton(string maneuverId, Transform transform)
    {
        GameObject newDialButton = GameObject.Instantiate<GameObject>(ManeuverIconPrefab, transform);
        newDialButton.name = maneuverId;
        newDialButton.GetComponent<Button>().interactable = false;
    }

    private void CreateButton(KeyValuePair<ManeuverHolder, MovementComplexity> dialInfo, Transform transform)
    {
        GameObject newDialButton = GameObject.Instantiate<GameObject>(ManeuverIconPrefab, transform);
        newDialButton.name = dialInfo.Key.ToString();
        newDialButton.GetComponentInChildren<Text>().text = dialInfo.Key.GetUiChar().ToString();

        Button button = newDialButton.GetComponent<Button>();

        ColorBlock colors = button.colors;
        colors.highlightedColor = ColorFromComplexity(dialInfo.Value);
        colors.pressedColor = colors.highlightedColor;
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = colors.highlightedColor;
        colors.normalColor = colors.highlightedColor;
        colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 150f / 255f);

        button.colors = colors;
        button.interactable = !IsDisabled;
    }

    private Color ColorFromComplexity(MovementComplexity complexity)
    {
        Color result = new Color();

        switch (complexity)
        {
            case MovementComplexity.Easy:
                result = Editions.Edition.Current.MovementEasyColor;
                break;
            case MovementComplexity.Normal:
                result = Color.white;
                break;
            case MovementComplexity.Complex:
                result = Color.red;
                break;
            default:
                break;
        }

        return result;
    }

    private void SetRowWidth(GameObject newIconsRow)
    {
        RectTransform rowRect = newIconsRow.GetComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(CalculateWidth(), IconSize);
    }

    private float CalculateWidth()
    {
        return (ColumnsSpecial == 0) ? IconSize * 6 + SpaceSize : IconSize * 6 + SpaceSize + IconSize * ColumnsSpecial;
    }

    private void UpdateRowsCount()
    {
        if (DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.ReverseStraight) && !DialToShow.Any(n => n.Key.Bearing == ManeuverBearing.Stationary))
        {
            Rows--;
        }
    }

    private void SetIconsHolderHeight()
    {
        RectTransform iconsRect = ManeuverIconsHolder.GetComponent<RectTransform>();
        iconsRect.sizeDelta = new Vector2(iconsRect.sizeDelta.x, IconSize * Rows);
    }

    private void SetFinalSize()
    {
        RectTransform rect = this.transform.GetComponent<RectTransform>();
        Width = CalculateWidth();
        Height = IconSize * Rows;
        rect.sizeDelta = new Vector2(Width, Height);
    }
}
