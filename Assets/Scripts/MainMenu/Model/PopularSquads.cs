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
        private static JSONObject Data;

        public static void LoadPopularSquads()
        {
            Global.Instance.StartCoroutine(LoadPopularArchetypesAsync());
        }

        public static IEnumerator LoadPopularArchetypesAsync()
        {
            UnityWebRequest www;
            www = UnityWebRequest.Get("https://flycasualdataserver.azurewebsites.net/api/populararchetypes");
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);

            Data = new JSONObject(www.downloadHandler.text);

            HideLoadingStub();
            ShowListOfArchetypes();
        }

        private static void HideLoadingStub()
        {
            GameObject loadingPanel = GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsPanel").Find("Loading").gameObject;
            loadingPanel.SetActive(false);
        }

        public static void ShowListOfArchetypes()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/ArchetypePanel", typeof(GameObject));
            GameObject archetypesPanel = GameObject.Find("UI/Panels").transform.Find("BrowsePopularSquadsPanel").Find("Scroll View/Viewport/Content").gameObject;

            RectTransform archetypesPanelRectTransform = archetypesPanel.GetComponent<RectTransform>();
            Vector3 currentPosition = new Vector3(archetypesPanelRectTransform.sizeDelta.x / 2 + FREE_SPACE, -FREE_SPACE, archetypesPanel.transform.localPosition.z);

            foreach (Transform transform in archetypesPanel.transform)
            {
                GameObject.Destroy(transform.gameObject);
            }

            archetypesPanelRectTransform.sizeDelta = new Vector2(archetypesPanelRectTransform.sizeDelta.x, 0);
            GameObject.Find("UI/Panels").transform.Find("ModsPanel").Find("Scroll View").GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;

            foreach (JSONObject archetype in Data.list)
            {
                GameObject archetypeRecord;

                archetypeRecord = MonoBehaviour.Instantiate(prefab, archetypesPanel.transform);
                archetypeRecord.transform.localPosition = currentPosition;
                archetypeRecord.name = archetype["name"].str;

                archetypeRecord.transform.Find("Name").GetComponent<Text>().text = archetype["name"].str;

                currentPosition = new Vector3(currentPosition.x, currentPosition.y - 120f - FREE_SPACE, currentPosition.z);
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
                    Debug.Log(shipIcons);
                    archetypeRecord.transform.Find("Ships").GetComponent<Text>().fontSize = 90; // 150 for 1 row
                }
                archetypeRecord.transform.Find("Ships").GetComponent<Text>().text = shipIcons;
            }
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
    }
}

