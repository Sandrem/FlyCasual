using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RosterBuilderUI : MonoBehaviour {

    public void AddShip(int playerNo)
    {
        RosterBuilder.TryAddShip(Tools.IntToPlayer(playerNo));
    }

    public void OnPlayerFactionChanged()
    {
        RosterBuilder.PlayerFactonChange();
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text;
    }

    public void PasteFromClipboard()
    {
        GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text = GUIUtility.systemCopyBuffer;
    }

}
