using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;
using UnityEngine.UI;

public class GuessManeuverScript : MonoBehaviour
{
    public static ManeuverBearing BearingChosen { get; private set; }
    public static ManeuverSpeed SpeedChosen { get; private set; }

    public static Action<ManeuverBearing, ManeuverSpeed> Callback { get; private set; }

    private GameObject CurrentBearingButton;
    private GameObject CurrentSpeedButton;

    private static GuessManeuverScript Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BearingButtonIsPressed(GameObject buttonGO)
    {
        SetColor(CurrentBearingButton, isHighlighted:false);
        CurrentBearingButton = buttonGO;
        SetColor(CurrentBearingButton, isHighlighted: true);

        BearingChosen = (ManeuverBearing)Enum.Parse(typeof(ManeuverBearing), buttonGO.name);
    }

    private static void SetColor(GameObject buttonGo, bool isHighlighted)
    {
        if (buttonGo != null)
        {
            Button oldButton = buttonGo.GetComponent<Button>();
            ColorBlock colorBlock = oldButton.colors;
            colorBlock.normalColor = (isHighlighted) ? new Color(0, 0, 1, 1) : new Color(1, 1, 1, 200f / 255f);
            oldButton.colors = colorBlock;
        }
    }

    public void SpeedButtonIsPressed(GameObject buttonGO)
    {
        SetColor(CurrentSpeedButton, isHighlighted: false);
        CurrentSpeedButton = buttonGO;
        SetColor(CurrentSpeedButton, isHighlighted: true);

        SpeedChosen = (ManeuverSpeed)Enum.Parse(typeof(ManeuverSpeed), buttonGO.name);
    }

    public void OkButtonIsPressed()
    {
        if (BearingChosen == default || SpeedChosen == default)
        {
            Messages.ShowError("Please, select bearing and speed first");
        }
        else
        {
            this.gameObject.SetActive(false);
            //Messages.ShowInfo(BearingChosen + " " + SpeedChosen);
            Callback(BearingChosen, SpeedChosen);
        }
    }

    public static void Initialize(Action<ManeuverBearing, ManeuverSpeed> callback)
    {
        if (Instance == null) Instance = GameObject.Find("UI").transform.Find("GuessManeuverPanel").GetComponent<GuessManeuverScript>();

        Instance.gameObject.SetActive(true);
        foreach (Button button in Instance.transform.Find("Center").GetComponentsInChildren<Button>())
        {
            SetColor(button.gameObject, isHighlighted: false);
        }

        Callback = callback;
    }
}
