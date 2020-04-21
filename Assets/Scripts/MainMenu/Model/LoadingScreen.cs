using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class LoadingScreen
{
    public static bool DontWaitFewSeconds;

    private static readonly float SECONDS_TO_WAIT = 4f;
    private static bool IsReady;
    private static Action Callback;

    private static readonly List<string> Tips = new List<string>()
    {
        // General strategy tips
        "If you’re both jousting, one of you is wrong",
        "The best defense dice are the ones you don’t have to roll",
        "Focus is better than boosting in range 1",
        "Focus and target lock reduce variance",
        "When in doubt, focus",
        "Dial in for where the enemy will be, not where they are now",
        "Your green dice will betray you",
        "You will miss 100% of the shots you do not take",
        "Obstacle/ship placement is turn zero",
        "What is your desired endgame?",
        "Try to block opponent’s aces",
        "In every matchup, there is a beatdown player, and a control player. Actions can be taken to change one's role in a situation...",
        "Stop blaming dice",
        "Think about the next turn when choosing your maneuver",
        "Sometimes bumping is good",
        "Sometimes flight through an obstacle is better then flight out of combat zone for few turns",
        "Sometimes the best idea is to fly in tight formation",
        "Don't install too many upgrades on a ship",

        // Sun Tzu
        "If you know the enemy and know yourself, you need not fear the result of a hundred battles. (Sun Tzu)",
        "All warfare is based on deception. (Sun Tzu)",
        "In the midst of chaos, there is also opportunity. (Sun Tzu)",
        "If your enemy is in superior strength, evade him. (Sun Tzu)",
        "If your enemy's forces are united, separate them. (Sun Tzu)",
        "To know your Enemy, you must become your Enemy. (Sun Tzu)",
        "He will win who knows when to fight and when not to fight. (Sun Tzu)",
        "Know yourself and you will win all battles. (Sun Tzu)",
        "Tactics without strategy is the noise before defeat", // Author is unknown, but this is in style of Sun Tzu

        // Controls
        "To measure distance to a ship: click Right Mouse Button on it",
        "To rotate a ship/obstacle during setup: use Q/E buttons",
        "Precise rotation during setup: CTRL+Q / CTRL+E",
        "To change angle of camera: hold Right Mouse Button and move mouse",
        "To change 3D/2D view: press TAB button",
        "To save/load squads: click SQUADS in squad builder",
        "To activate Skip button: press SPACE",

        // Development
        "Please report bugs on GitHub, with screenshots",

        // The AI doesn't cheat!
        "I repeat again: The die rolls are fair",
        "No, the AI doesn't look at your dials",

        // Quotes
        "Try spinning - that's a good trick",
        "Do, or do not. There is no try.",
        "May the Force be with you!",
        "01110010 01101111 01100111 01100101 01110010\n01110010 01101111 01100111 01100101 01110010",  // "roger roger" in binary
    };

    public static void Show()
    {
        Callback = null;
        IsReady = false;

        Transform loadingScreen = GameObject.Find("GlobalUI").transform.Find("LoadingScreen");
        loadingScreen.GetComponent<Image>().sprite = GetRandomSplashScreen();

        Text LoadingText = loadingScreen.Find("LoadingInfoPanel").GetComponentInChildren<Text>();
        if (loadingScreen != null) loadingScreen.gameObject.SetActive(true);

        if (!DontWaitFewSeconds)
        {
            LoadingText.text = Tips[UnityEngine.Random.Range(0, Tips.Count)];
            Global.Instance.StartCoroutine(WaitFewSeconds());
        }
        else
        {
            LoadingText.text = "Loading...";
            WaitingIsFinished();
        }
    }

    private static Sprite GetRandomSplashScreen()
    {
        List<Sprite> spritesArray = new List<Sprite>();
        spritesArray.AddRange(
            Resources.LoadAll<Sprite>("Sprites/Backgrounds/MainMenu/")
                .Where(n => n.name != "_RANDOM")
                .ToList()
        );
        spritesArray.AddRange(
            Resources.LoadAll<Sprite>("Sprites/Backgrounds/SplashScreens/")
                .ToList()
        );
        return spritesArray[UnityEngine.Random.Range(0, spritesArray.Count)];
    }

    private static IEnumerator WaitFewSeconds()
    {
        yield return new WaitForSeconds(SECONDS_TO_WAIT);
        WaitingIsFinished();
    }

    private static void WaitingIsFinished()
    {
        IsReady = true;

        if (Callback != null)
        {
            Hide();
        }
    }

    private static void Hide()
    {
        Transform loadingScreen = GameObject.Find("GlobalUI").transform.Find("LoadingScreen");
        if (loadingScreen != null) loadingScreen.gameObject.SetActive(false);

        Callback();
    }

    public static void NextSceneIsReady(Action callback)
    {
        Callback = callback;

        if (IsReady) Hide();
    }
}

