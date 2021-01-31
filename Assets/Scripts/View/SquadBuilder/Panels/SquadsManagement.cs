using Editions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public class SquadsManagement
    {
        private SquadBuilderView View;

        public SquadsManagement(SquadBuilderView squadBuilderView)
        {
            View = squadBuilderView;
        }

        public void PrepareSaveSquadronPanel()
        {
            GameObject.Find("UI/Panels/SaveSquadronPanel/Panel/Name/InputField").GetComponent<InputField>().text = Global.SquadBuilder.CurrentSquad.Name;
        }

        public void OpenImportExportPanel(bool isImport)
        {
            MainMenu.CurrentMainMenu.ChangePanel("ImportExportPanel");

            GameObject.Find("UI/Panels/ImportExportPanel/Content/InputField").GetComponent<InputField>().text = (isImport) ? "" : Global.SquadBuilder.CurrentSquad.GetSquadInJson().ToString();
            GameObject.Find("UI/Panels/ImportExportPanel/BottomPanel/ImportButton").SetActive(isImport);
        }

        private List<JSONObject> GetSavedSquadsJsons()
        {
            List<JSONObject> savedSquadsJsons = new List<JSONObject>();

            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToSavedSquadrons;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                string content = File.ReadAllText(filePath);
                JSONObject squadJson = new JSONObject(content);

                if (Global.SquadBuilder.CurrentSquad.SquadFaction == Faction.None || Edition.Current.XwsToFaction(squadJson["faction"].str) == Global.SquadBuilder.CurrentSquad.SquadFaction)
                {
                    squadJson.AddField("filename", new FileInfo(filePath).Name);
                    savedSquadsJsons.Add(squadJson);
                }
            }

            return savedSquadsJsons;
        }

        public void ShowListOfSavedSquadrons(List<JSONObject> squadsJsonsList)
        {
            SetNoSavedSquadronsMessage(squadsJsonsList.Count == 0);

            float FREE_SPACE = 25;

            Transform contentTransform = GameObject.Find("UI/Panels/BrowseSavedSquadsPanel/Scroll View/Viewport/Content").transform;

            SquadBuilderView.DestroyChildren(contentTransform);

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SavedSquadronPanel", typeof(GameObject));

            //RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
            //Vector3 currentPosition = new Vector3(0, -FREE_SPACE, contentTransform.localPosition.z);

            foreach (var squadList in squadsJsonsList)
            {
                GameObject SquadListRecord;

                SquadListRecord = MonoBehaviour.Instantiate(prefab, contentTransform);

                SquadListRecord.transform.Find("Name").GetComponent<Text>().text = squadList["name"].str;

                Text descriptionText = SquadListRecord.transform.Find("Description").GetComponent<Text>();
                RectTransform descriptionRectTransform = SquadListRecord.transform.Find("Description").GetComponent<RectTransform>();
                if (squadList.HasField("description"))
                {
                    descriptionText.text = squadList["description"].str.Replace("\\\"", "\"");
                }
                else
                {
                    descriptionText.text = "No description";
                }

                float descriptionPreferredHeight = descriptionText.preferredHeight;
                descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, descriptionPreferredHeight);

                SquadListRecord.transform.Find("PointsValue").GetComponent<Text>().text = squadList["points"].i.ToString();

                SquadListRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    SquadListRecord.GetComponent<RectTransform>().sizeDelta.x,
                    15 + 70 + 10 + descriptionPreferredHeight + 10 + 55 + 10
                );

                SquadListRecord.name = squadList["filename"].str;

                SquadListRecord.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(delegate { DeleteSavedSquadAndRefresh(SquadListRecord.name); });
                SquadListRecord.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener(delegate { LoadSavedSquadAndReturn(GetSavedSquadJson(SquadListRecord.name)); });
            }

            SquadBuilderView.OrganizePanels(contentTransform, FREE_SPACE);
        }

        private JSONObject GetSavedSquadJson(string fileName)
        {
            JSONObject squadJson = null;

            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToSavedSquadrons;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = directoryPath + "/" + fileName;

            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                squadJson = new JSONObject(content);
            }

            return squadJson;
        }

        private void SetNoSavedSquadronsMessage(bool isActive)
        {
            GameObject.Find("UI/Panels/BrowseSavedSquadsPanel/NoSavedSquads").SetActive(isActive);
        }

        private void DeleteSavedSquadFile(string fileName)
        {
            string filePath = Application.persistentDataPath + "/" + Edition.Current.Name + "/SavedSquadrons/" + fileName;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // IMPORT / EXPORT

        public void BrowseSavedSquads()
        {
            List<JSONObject> sortedSavedSquadsJsons = GetSavedSquadsJsons();
            foreach (string autosaveName in new List<string>() { "Autosave", "Autosave (Player 2)", "Autosave (Player 1)" })
            {
                SetAutosavesOnTop(sortedSavedSquadsJsons, autosaveName);
            }
            ShowListOfSavedSquadrons(sortedSavedSquadsJsons);
        }

        private void SetAutosavesOnTop(List<JSONObject> jsonList, string autosaveName)
        {
            try
            {
                JSONObject autosaveJson = jsonList.Find(n => n["name"].str == autosaveName);
                if (autosaveJson != null)
                {
                    jsonList.Remove(autosaveJson);
                    jsonList.Insert(0, autosaveJson);
                }
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private void DeleteSavedSquadAndRefresh(string fileName)
        {
            if (Edition.Current.IsSquadBuilderLocked)
            {
                Messages.ShowError("This part of squad builder is disabled");
                return;
            }

            DeleteSavedSquadFile(fileName);
            BrowseSavedSquads();
        }

        public void LoadSavedSquadAndReturn(JSONObject squadJson)
        {
            Global.SquadBuilder.SquadLists[Global.SquadBuilder.CurrentPlayer].SetPlayerSquadFromImportedJson(squadJson);
            View.ReturnToSquadBuilder();
        }
    }
}
