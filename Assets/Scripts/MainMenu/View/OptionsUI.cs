using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public class OptionsUI : MonoBehaviour {

    public GameObject PlaymatSelector { get; private set; }
    public static OptionsUI Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void OnClickPlaymatChange(GameObject playmatImage)
    {
        PlayerPrefs.SetString("PlaymatName", playmatImage.name);
        PlayerPrefs.Save();

        Options.Playmat = playmatImage.name;

        PlaymatSelector.transform.position = playmatImage.transform.position;
    }

    public void RestoreDefaults()
    {
        Options.Playmat = "3DSceneHoth";
        Options.ChangeParameterValue("Music Volume", 0.25f);
        Options.ChangeParameterValue("SFX Volume", 0.25f);
        Options.ChangeParameterValue("Animation Speed", 0.25f);
        Options.ChangeParameterValue("Maneuver Speed", 0.25f);

        Options.InitializePanel();
    }

    public void InitializeOptionsPanel()
    {
        CategorySelected(GameObject.Find("UI/Panels/OptionsPanel/Content/CategoriesPanel/PlayerButton"));
    }

    public void CategorySelected(GameObject categoryGO)
    {
        ClearOptionsView();
        categoryGO.GetComponent<Image>().color = new Color(0, 0.5f, 1, 100f/256f);

        switch (categoryGO.GetComponentInChildren<Text>().text)
        {
            case "Avatar":
                ShowAvatarSelection();
                break;
            case "Player":
                ShowPlayerView();
                break;
            case "Animations":
                ShowViewSimple("Animations");
                break;
            case "Sounds":
                ShowViewSimple("Sounds");
                break;
            case "Playmat":
                ShowPlaymatSelection();
                break;
            case "Background":
                ShowBackgroundSelection();
                break;
            case "Extra":
                ShowExtraView();
                break;
            default:
                break;
        }
    }

    private void ShowBackgroundSelection()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/BackgroundSelectionViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject imageListParent = Instantiate(prefab, parentTransform);

        foreach (Sprite backgroundImage in Resources.LoadAll<Sprite>("Sprites/Backgrounds/MainMenu"))
        {
            GameObject backgroundGO = new GameObject(backgroundImage.name);
            backgroundGO.AddComponent<Image>().sprite = backgroundImage;
            backgroundGO.transform.SetParent(imageListParent.transform);
            backgroundGO.transform.localScale = Vector3.one;

            Button button = backgroundGO.AddComponent<Button>();
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = new Color(1, 1, 1, 200f / 256f);
            button.colors = buttonColors;

            button.onClick.AddListener(() =>
            {
                MainMenu.SetBackground(Resources.Load<Sprite>("Sprites/Backgrounds/MainMenu/" + backgroundImage.name));
            });
        }
    }

    private void ShowPlaymatSelection()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/PlaymatSelectionViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject imageListParent = Instantiate(prefab, parentTransform);

        foreach (Sprite playmatSprite in Resources.LoadAll<Sprite>("Playmats/Thumbnails"))
        {
            GameObject playmatPreviewGO = new GameObject(playmatSprite.name);
            playmatPreviewGO.AddComponent<Image>().sprite = playmatSprite;
            playmatPreviewGO.transform.SetParent(imageListParent.transform);
            playmatPreviewGO.transform.localScale = Vector3.one;

            Button button = playmatPreviewGO.AddComponent<Button>();
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = new Color(1, 1, 1, 200f / 256f);
            button.colors = buttonColors;

            button.onClick.AddListener(() =>
            {
                string playmatName = playmatSprite.name.Replace("Thumbnail", "").Replace("Playmat", "");
                PlayerPrefs.SetString("PlaymatName", playmatName);
                PlayerPrefs.Save();

                Options.Playmat = playmatName;
            });
        }
    }

    private void ShowAvatarSelection()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/AvatarSelectionViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject imageListParent = Instantiate(prefab, parentTransform);
        imageListParent.name = "AvatarSelectionViewPanel";

        /*List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList.FirstEdition", StringComparison.Ordinal))
            .ToList();*/

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList.FirstEdition", StringComparison.Ordinal) || String.Equals(t.Namespace, "UpgradesList.SecondEdition", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {

            if (type.MemberType == MemberTypes.NestedType) continue;

            GenericUpgrade newUpgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(type);
            if (newUpgradeContainer.UpgradeInfo.Name != null)
            {
                //  && newUpgradeContainer.Avatar.AvatarFaction == CurrentAvatarsFaction
                if (newUpgradeContainer.Avatar != null) AddAvailableAvatar(newUpgradeContainer);
            }
        }
    }

    private void AddAvailableAvatar(GenericUpgrade avatarUpgrade)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/MainMenu/AvatarImage", typeof(GameObject));
        GameObject avatarPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel/AvatarSelectionViewPanel").transform);

        avatarPanel.name = avatarUpgrade.GetType().ToString();

        AvatarFromUpgrade avatar = avatarPanel.GetComponent<AvatarFromUpgrade>();
        avatar.Initialize(avatarUpgrade.GetType().ToString(), ChangeAvatar);

        /*if (avatarUpgrade.GetType().ToString() == Options.Avatar)
        {
            SetAvatarSelected(avatarPanel.transform.position);
        }*/
    }

    private void ChangeAvatar(string avatarName)
    {
        Options.Avatar = avatarName;
        Options.ChangeParameterValue("Avatar", avatarName);

        // SetAvatarSelected(GameObject.Find("UI/Panels/AvatarsPanel/ContentPanel/" + avatarName).transform.position);
    }

    private void ShowPlayerView()
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/PlayerViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject panel = Instantiate(prefab, parentTransform);

        InputField nameText = panel.transform.Find("NameInputPanel/InputField").GetComponent<InputField>();
        nameText.text = Options.NickName;
        nameText.onEndEdit.AddListener(delegate { MainMenu.CurrentMainMenu.ChangeNickName(nameText.text); });

        panel.transform.Find("TitleInputPanel/InputField").GetComponent<InputField>().text = Options.Title;
    }

    private void ShowExtraView()
    {
        /*Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/ExtraViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        GameObject panel = Instantiate(prefab, parentTransform);*/

        // Foreach class in namespace
        // Create panel with info and control of on/off mode
    }

    private void ShowViewSimple(string name)
    {
        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Options/" + name + "ViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        //GameObject panel = 
        Instantiate(prefab, parentTransform);
    }

    private void ClearOptionsView()
    {
        Transform categoryTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/CategoriesPanel").transform;
        foreach (Transform transform in categoryTransform.transform)
        {
            transform.GetComponent<Image>().color = new Color(0, 0.5f, 1, 0);
        }

        Transform parentTransform = GameObject.Find("UI/Panels/OptionsPanel/Content/ContentViewPanel").transform;
        foreach (Transform transform in parentTransform.transform)
        {
            Destroy(transform.gameObject);
        }
    }

}
