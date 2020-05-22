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
        "In every matchup, there is a beatdown player, and a control player",
        "Stop blaming dice",
        "Think about the next turn when choosing your maneuver",
        "Sometimes bumping is good",
        "Practice formation flying",
        "Practice, practice, practice",
        "Learn the rule of 11",
        "Don't install too many upgrades on a ship",
        "Bring the right obstacles for your list and a plan on how to use them",
        "Fly a ship according to its role: Jouster, Flanker, Tank, Support",
        "An extra ship is always better value than a couple of upgrades",

        // Sun Tzu
        "If you know the enemy and know yourself, you need not fear the result of a hundred battles",
        "In the midst of chaos, there is also opportunity",
        "If your enemy is in superior strength, evade him",
        "If your enemy's forces are united, separate them",
        "To know your Enemy, you must become your Enemy",
        "He will win who knows when to fight and when not to fight",
        "Know yourself and you will win all battles",
        "Tactics without strategy is the noise before defeat", // Author is unknown, but this is in style of Sun Tzu

        // Controls
        "To measure distance to a ship: click Right Mouse Button on it",
        "To rotate a ship/obstacle during setup: use Q/E buttons",
        "Precise rotation during setup: CTRL+Q / CTRL+E",
        "To change angle of camera: hold Right Mouse Button and move mouse",
        "To change 3D/2D view: press TAB button",
        "To save/load squads: click SQUADS in squad builder",
        "To activate Skip button: press SPACE",
        "You can import/export squads from popular squad builders in XWS format",
        "To see ID-numbers of ships: hold ALT",

        // Development
        "Please report bugs on GitHub, with screenshots",
        "Online multiplayer is in development",

        // The AI doesn't cheat!
        "I repeat again: The die rolls are fair",
        "No, the AI doesn't look at your dials",

        // Specially for FFG
        "You definitely need to buy more miniatures",

        // Quotes
        "Try spinning - that's a good trick",
        "Do, or do not. There is no try.",
        "May the Force be with you!",
        "01110010 01101111 01100111 01100101 01110010\n01110010 01101111 01100111 01100101 01110010",  // "roger roger" in binary
        "Permission to jump in an X-wing and blow something up?"
    };

    public static void Show()
    {
        Callback = null;
        IsReady = false;

        Transform loadingScreen = GameObject.Find("GlobalUI").transform.Find("LoadingScreen");
        loadingScreen.transform.Find("BackgroundImage").GetComponent<Image>().sprite = GetRandomSplashScreen();

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

