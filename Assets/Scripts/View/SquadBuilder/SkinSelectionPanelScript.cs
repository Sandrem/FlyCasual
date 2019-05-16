using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public class SkinSelectionPanelScript : MonoBehaviour
{
    public Text SkinName;
    public Text PilotName;
    public Text UpgradesList;
    public RawImage SkinImage;

    private GenericShip Ship;
    private int Number;

    // Start is called before the first frame update
    void Start()
    {
        PilotName.text = Ship.PilotInfo.PilotName;

        string upgradesListText = "";
        foreach (GenericUpgrade upgrade in Ship.UpgradeBar.GetUpgradesAll())
        {
            upgradesListText += upgrade.UpgradeInfo.Name + "\n";
        }
        UpgradesList.text = upgradesListText;

        SkinName.text = "<< " + Ship.ModelInfo.SkinName + " >>";

        RenderTexture cameraTexurePrefab = Resources.Load<RenderTexture>("Textures/CameraSkinPreviewTexture");
        RenderTexture cameraTexure = Instantiate<RenderTexture>(cameraTexurePrefab);

        GameObject skinPreviewGroupPrefab = Resources.Load<GameObject>("Prefabs/SquadBuilder/SkinPreviewGroup");
        GameObject skinPreview = GameObject.Instantiate<GameObject>(skinPreviewGroupPrefab, GameObject.Find("PreviewsHolder").transform);
        skinPreview.transform.localPosition = new Vector3(Number * 30, 0, 0);

        GameObject shipHolder = skinPreview.transform.Find("ShipHolder").gameObject;
        GameObject shipModelPrefab = Ship.GetShipModelPrefab();
        GameObject shipModel = GameObject.Instantiate<GameObject>(shipModelPrefab, shipHolder.transform);

        Ship.SetShipSkin(shipModel.transform);

        Camera camera = skinPreview.GetComponentInChildren<Camera>();
        camera.targetTexture = cameraTexure;

        SkinImage.texture = cameraTexure;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(GenericShip ship, int number)
    {
        Ship = ship;
        Number = number;
    }
}
