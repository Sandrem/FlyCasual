using Editions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public static class PopularSquads
    {
        public const float FREE_SPACE = 25f;
        public static string LastChosenFaction { get; set; }
        public static int SelectedSquadId { get; private set; }

        private static JSONObject Data;
        private static JSONObject VariantsData;
        

        public static void LoadPopularSquads()
        {
            ClearPage("BrowsePopularSquadsPanel");

            if (Edition.Current is Editions.SecondEdition)
            {
                Global.Instance.StartCoroutine(LoadPopularArchetypesAsync());
            }
            else
            {
                Messages.ShowError("Only for Second Edition");
            }
        }

        private static void ClearPage(string pageName)
        {
            GameObject archetypesPanel = GameObject.Find("UI/Panels").transform.Find(pageName).Find("Scroll View/Viewport/Content").gameObject;
            RectTransform contentTransform = archetypesPanel.GetComponent<RectTransform>();
            contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, 0, contentTransform.localPosition.z);
            foreach (Transform transform in contentTransform.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            GameObject loadingPanel = GameObject.Find("UI/Panels").transform.Find(pageName).Find("Loading").gameObject;
            loadingPanel.SetActive(true);
        }

        public static IEnumerator LoadPopularArchetypesAsync()
        {
            UnityWebRequest www;
            www = UnityWebRequest.Get("https://flycasualdataserver.azurewebsites.net/api/populararchetypes");
            yield return www.SendWebRequest();

            Data = new JSONObject(www.downloadHandler.text);

            HideLoadingStub("BrowsePopularSquadsPanel");
            if (Data.list != null)
            {
                ShowListOfArchetypes();
                SetFaction(LastChosenFaction);
            }
            else
            {
                Messages.ShowError("Cannot load data");
            }
        }

        private static void HideLoadingStub(string pageName)
        {
            GameObject loadingPanel = GameObject.Find("UI/Panels").transform.Find(pageName).Find("Loading").gameObject;
            loadingPanel.SetActive(false);
        }

        public static void ShowListOfArchetypes()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/ArchetypePanel", typeof(GameObject));
            GameObject archetypesPanel = GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsPanel").Find("Scroll View/Viewport/Content").gameObject;

            RectTransform archetypesPanelRectTransform = archetypesPanel.GetComponent<RectTransform>();

            foreach (Transform transform in archetypesPanel.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            archetypesPanelRectTransform.sizeDelta = new Vector2(archetypesPanelRectTransform.sizeDelta.x, 0);
            GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsPanel").Find("Scroll View").GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;

            foreach (JSONObject archetype in Data.list)
            {
                GameObject archetypeRecord;

                archetypeRecord = MonoBehaviour.Instantiate(prefab, archetypesPanel.transform);
                archetypeRecord.name = archetype["name"].str;

                archetypeRecord.transform.Find("Name").GetComponent<Text>().text = archetype["name"].str;

                archetypesPanelRectTransform.sizeDelta = new Vector2(archetypesPanelRectTransform.sizeDelta.x, archetypesPanelRectTransform.sizeDelta.y + 120f + FREE_SPACE);

                string factionText = FactionToChar(archetype["faction"].str.ToLower()).ToString();
                archetypeRecord.transform.Find("Faction").GetComponent<Text>().text = factionText;

                string shipIcons = "";
                foreach (JSONObject ship in archetype["ships"].list)
                {
                    ShipRecord shipRecord = SquadBuilder.AllShips.FirstOrDefault(n => n.ShipName == ship.str);
                    if (shipRecord != null)
                    {
                        shipIcons += shipRecord.Instance.ShipIconLetter;
                    }
                }
                if (shipIcons.Length > 5)
                {
                    shipIcons = shipIcons.Insert((shipIcons.Length - shipIcons.Length % 2) / 2, "\n");
                    archetypeRecord.transform.Find("Ships").GetComponent<Text>().fontSize = 90; // 150 for 1 row
                }
                archetypeRecord.transform.Find("Ships").GetComponent<Text>().text = shipIcons;

                archetypeRecord.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener
                (
                    () =>
                    {
                        SelectedSquadId = int.Parse(archetype["id"].str);
                        MainMenu.CurrentMainMenu.ChangePanel("BrowsePopularSquadsVariantsPanel"); 
                    }
                ); ;
            }
        }

        public static void SetFaction(string factionChar)
        {
            LastChosenFaction = factionChar;

            GameObject archetypesPanel = GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsPanel").Find("Scroll View/Viewport/Content").gameObject;
            RectTransform contentTransform = archetypesPanel.GetComponent<RectTransform>();

            foreach (Transform squadPanel in contentTransform.transform)
            {
                squadPanel.gameObject.SetActive(factionChar == "All" || squadPanel.Find("Faction").GetComponent<Text>().text == factionChar);
            }

            OrganizePanels(contentTransform, FREE_SPACE);
        }

        private static char FactionToChar(string faction)
        {
            switch (faction)
            {
                case "rebel":
                    return '!';
                case "imperial":
                    return '@';
                case "scum":
                    return '#';
                case "resistance":
                    return '-';
                case "firstorder":
                    return '+';
                case "republic":
                    return '/';
                case "separatists":
                    return '.';
                default:
                    return ' ';
            }
        }

        public static void LoadPopularSquadsVariants()
        {
            ClearPage("BrowsePopularSquadsVariantsPanel");

            Global.Instance.StartCoroutine(LoadPopularSquadsVariantsAsync());
        }

        public static IEnumerator LoadPopularSquadsVariantsAsync()
        {
            UnityWebRequest www;
            www = UnityWebRequest.Get("https://flycasualdataserver.azurewebsites.net/api/ArchetypeVariants/" + SelectedSquadId);
            yield return www.SendWebRequest();

            VariantsData = new JSONObject(www.downloadHandler.text);

            HideLoadingStub("BrowsePopularSquadsVariantsPanel");
            if (VariantsData.list != null)
            {
                ShowListOfArchetypesVariants();
            }
            else
            {
                Messages.ShowError("Cannot load data");
            }
        }

        public static void ShowListOfArchetypesVariants()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SavedSquadronPanel", typeof(GameObject));
            GameObject archetypesPanel = GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsVariantsPanel").Find("Scroll View/Viewport/Content").gameObject;

            RectTransform contentTransform = archetypesPanel.GetComponent<RectTransform>();

            foreach (Transform transform in contentTransform.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            List<string> existingLists = new List<string>();

            foreach (var squadJson in VariantsData.list)
            {
                SquadList squadList = new SquadList(Players.PlayerNo.PlayerNone);
                
                JSONObject squadJsonFixed = new JSONObject(squadJson["json"].str);
                if (existingLists.Contains(squadJsonFixed.ToString())) continue;
                existingLists.Add(squadJsonFixed.ToString());

                SquadBuilder.SetPlayerSquadFromImportedJson(squadJsonFixed, squadList, delegate { });

                GameObject SquadListRecord;

                SquadListRecord = MonoBehaviour.Instantiate(prefab, contentTransform);

                SquadListRecord.transform.Find("Name").GetComponent<Text>().text = "Example"; //squadList["name"].str;

                Text descriptionText = SquadListRecord.transform.Find("Description").GetComponent<Text>();
                RectTransform descriptionRectTransform = SquadListRecord.transform.Find("Description").GetComponent<RectTransform>();
                if (squadJson.HasField("json"))
                {
                    descriptionText.text = SquadBuilder.GetDescriptionOfSquadJson(squadJsonFixed).Replace("\\\"", "\"");
                }
                else
                {
                    descriptionText.text = "No description";
                }

                float descriptionPreferredHeight = descriptionText.preferredHeight;
                descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, descriptionPreferredHeight);

                SquadListRecord.transform.Find("PointsValue").GetComponent<Text>().text = squadList.Points.ToString();

                SquadListRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    SquadListRecord.GetComponent<RectTransform>().sizeDelta.x,
                    15 + 70 + 10 + descriptionPreferredHeight + 10 + 55 + 10
                );

                SquadListRecord.transform.Find("DeleteButton").gameObject.SetActive(false);
                SquadListRecord.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener(delegate { SquadBuilder.LoadSavedSquadAndReturn(squadJsonFixed); });
            }

            OrganizePanels(contentTransform, FREE_SPACE);
        }

        private static void OrganizePanels(Transform contentTransform, float freeSpace)
        {
            contentTransform.localPosition = new Vector3(contentTransform.localPosition.x, 0, contentTransform.localPosition.z);

            float totalHeight = 0;
            foreach (Transform transform in contentTransform)
            {
                if (transform.gameObject.activeSelf && transform.name != "DestructionIsPlanned")
                {
                    totalHeight += transform.GetComponent<RectTransform>().sizeDelta.y + freeSpace;
                }
            }
            RectTransform contRect = contentTransform.GetComponent<RectTransform>();
            contRect.sizeDelta = new Vector2(contRect.sizeDelta.x, totalHeight + 25);

            totalHeight = 25;
            foreach (Transform transform in contentTransform)
            {
                if (transform.gameObject.activeSelf && transform.name != "DestructionIsPlanned")
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, -totalHeight);
                    totalHeight += transform.GetComponent<RectTransform>().sizeDelta.y + freeSpace;
                }
            }
        }
    }
}

