using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mods;

public class ModsUI : MonoBehaviour {

    public const float FREE_SPACE = 20f;

    public void Start()
    {
        ModsManager.UI = this;
    }

    public void InitializePanel()
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/ModPanel", typeof(GameObject));
        GameObject ModsPanel = GameObject.Find("UI/Panels").transform.Find("ModsPanel").Find("Scroll View/Viewport/Content").gameObject;

        RectTransform modsPanelRectTransform = ModsPanel.GetComponent<RectTransform>();
        Vector3 currentPosition = new Vector3(ModsPanel.transform.position.x - modsPanelRectTransform.sizeDelta.x/2 + FREE_SPACE, ModsPanel.transform.position.y - FREE_SPACE, ModsPanel.transform.position.z);

        foreach (var mod in ModsManager.Mods)
        {
            Transform existingModeRecord = ModsPanel.transform.Find(mod.Key.ToString());
            GameObject ModRecord;

            if (existingModeRecord == null)
            {
                ModRecord = MonoBehaviour.Instantiate(prefab, ModsPanel.transform);
                ModRecord.transform.position = currentPosition;
                ModRecord.name = mod.Key.ToString();

                ModRecord.transform.Find("Label").GetComponent<Text>().text = mod.Value.Name;

                Text description = ModRecord.transform.Find("Text").GetComponent<Text>();
                description.text = mod.Value.Description;
                RectTransform descriptionRectTransform = description.GetComponent<RectTransform>();
                descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, description.preferredHeight);

                RectTransform modRecordRectTransform = ModRecord.GetComponent<RectTransform>();
                modRecordRectTransform.sizeDelta = new Vector2(modRecordRectTransform.sizeDelta.x, modRecordRectTransform.sizeDelta.y + description.preferredHeight);

                currentPosition = new Vector3(currentPosition.x, currentPosition.y - modRecordRectTransform.sizeDelta.y - FREE_SPACE, currentPosition.z);
                modsPanelRectTransform.sizeDelta = new Vector2(modsPanelRectTransform.sizeDelta.x, modsPanelRectTransform.sizeDelta.y + modRecordRectTransform.sizeDelta.y + FREE_SPACE);
            }
            else
            {
                ModRecord = existingModeRecord.gameObject;
            }

            ModRecord.transform.Find("Toggle").GetComponent<Toggle>().isOn = ModsManager.Mods[mod.Key].IsOn;
        }
    }

}
