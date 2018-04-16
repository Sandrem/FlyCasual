using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using UnityEngine.UI;
using SquadBuilderNS;
using System.Linq;
using Players;
using System.Reflection;
using System;
using Upgrade;

public partial class MainMenu : MonoBehaviour {

    string NewVersionUrl;

    public static MainMenu CurrentMainMenu;

    // Use this for initialization
    void Start ()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        CurrentMainMenu = this;
        SetCurrentPanel();

        DontDestroyOnLoad(GameObject.Find("GlobalUI").gameObject);

        SetBackground();
        ModsManager.Initialize();
        Options.ReadOptions();
        Options.UpdateVolume();
        UpdateVersionInfo();
        UpdatePlayerInfo();
        CheckUpdates();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnUpdateAvailableClick()
    {
        Application.OpenURL(NewVersionUrl);
    }

    private void UpdateVersionInfo()
    {
        GameObject.Find("UI/Panels/MainMenuPanel/Version/Version Text").GetComponent<Text>().text = Global.CurrentVersion;
    }

    private void UpdatePlayerInfo()
    {
        AvatarFromUpgrade script = GameObject.Find("UI/Panels/MainMenuPanel/PlayerInfoPanel/AvatarImage").GetComponent<AvatarFromUpgrade>();
        script.Initialize(Options.Avatar);
    }

    private void SetBackground()
    {
        GameObject.Find("UI/BackgroundImage").GetComponent<Image>().sprite = GetRandomBackground();
    }

    public static Sprite GetRandomBackground()
    {
        UnityEngine.Object[] sprites = Resources.LoadAll("Sprites/Backgrounds/", typeof(Sprite));
        return (Sprite) sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }

    private void CheckUpdates()
    {
        int latestVersionInt = RemoteSettings.GetInt("UpdateLatestVersionInt", Global.CurrentVersionInt);
        if (latestVersionInt > Global.CurrentVersionInt)
        {
            string latestVersion    = RemoteSettings.GetString("UpdateLatestVersion", Global.CurrentVersion);
            string updateLink       = RemoteSettings.GetString("UpdateLink");
            ShowNewVersionIsAvailable(latestVersion, updateLink);
        }
    }

    // 0.3.2 UI

    public void CreateMatch()
    {
        string roomName = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Name").GetComponentInChildren<InputField>().text;
        string password = GameObject.Find("UI/Panels/CreateMatchPanel/Panel/Password").GetComponentInChildren<InputField>().text;
        Network.CreateMatch(roomName, password);
    }

    public void BrowseMatches()
    {
        Network.BrowseMatches();
    }

    public void JoinMatch(GameObject panel)
    {
        //Messages.ShowInfo("Joining room...");
        string password = panel.transform.Find("Password").GetComponentInChildren<InputField>().text;
        Network.JoinCurrentRoomByParameters(password);
    }

    public void CancelWaitingForOpponent()
    {
        Network.CancelWaitingForOpponent();
    }

    public void StartSquadBuilerMode(string modeName)
    {
        SquadBuilder.Initialize();
        SquadBuilder.SetCurrentPlayer(PlayerNo.Player1);
        SquadBuilder.SetPlayers(modeName);
        SquadBuilder.SetDefaultPlayerNames();
        ChangePanel("SelectFactionPanel");
    }

    public static void InitializeAvatars()
    {
        ClearAvatarsPanel();

        int count = 0;

        List<GenericUpgrade> AllUpgrades = new List<GenericUpgrade>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            if (type.MemberType == MemberTypes.NestedType) continue;

            GenericUpgrade newUpgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(type);
            if (newUpgradeContainer.Name != null)
            {
                if (newUpgradeContainer.AvatarOffset != Vector2.zero) AddAvailableAvatar(newUpgradeContainer, count++);
            }
        }
    }

    private static void ClearAvatarsPanel()
    {
        GameObject avatarsPanel = GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel").gameObject;
        foreach (Transform transform in avatarsPanel.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }

        Resources.UnloadUnusedAssets();
    }

    private static void AddAvailableAvatar(GenericUpgrade avatarUpgrade, int count)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/MainMenu/AvatarImage", typeof(GameObject));
        GameObject avatarPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel").transform);

        int row = count / 11;
        int column = count - row * 11;

        avatarPanel.transform.localPosition = new Vector2(20 + column * 120, -20 - row * 120);

        AvatarFromUpgrade avatar = avatarPanel.GetComponent<AvatarFromUpgrade>();
        avatar.Initialize(avatarUpgrade.GetType().ToString(), ChangeAvatar);
    }

    private static void ChangeAvatar(string avatarName)
    {
        Options.Avatar = avatarName;
        Options.ChangeParameterValue("Avatar", avatarName);
        Messages.ShowInfo("Avatar is changed");
    }

}
