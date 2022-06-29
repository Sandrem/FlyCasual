using Ship;
using System;
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
    private int SkinIndex;
    private GameObject ShipModel;
    private GameObject SkinPreview;

    private List<Texture> AvailableSkins;

    // Start is called before the first frame update
    void Start()
    {
        SetShipInformation();
        CreatePreviewGroup();
        CreateShipModel();
        ApplyDefaultSkin();
        PrepareCamera();
    }

    private void PrepareCamera()
    {
        RenderTexture cameraTexurePrefab = Resources.Load<RenderTexture>("Textures/CameraSkinPreviewTexture");
        RenderTexture cameraTexure = Instantiate<RenderTexture>(cameraTexurePrefab);
        Camera camera = SkinPreview.GetComponentInChildren<Camera>();
        camera.targetTexture = cameraTexure;
        SkinImage.texture = cameraTexure;
        if (Ship.ModelInfo.PreviewCameraPosition != default) camera.transform.localPosition = Ship.ModelInfo.PreviewCameraPosition;
        if (Ship.ModelInfo.PreviewScale != 0) camera.orthographicSize = Ship.ModelInfo.PreviewScale;
    }

    private void ApplyDefaultSkin()
    {
        AvailableSkins = Ship.GetAvailableSkins();
        SkinIndex = GetCurrentSkinIndex();
        ApplySkinChangeSkin(SkinIndex);
    }

    private void CreateShipModel()
    {
        GameObject shipHolder = SkinPreview.transform.Find("ShipHolder").gameObject;
        GameObject shipModelPrefab = Ship.GetShipModelPrefab();
        ShipModel = GameObject.Instantiate<GameObject>(shipModelPrefab, shipHolder.transform);
    }

    private void CreatePreviewGroup()
    {
        GameObject skinPreviewGroupPrefab = Resources.Load<GameObject>("Prefabs/SquadBuilder/SkinPreviewGroup");
        SkinPreview = GameObject.Instantiate<GameObject>(skinPreviewGroupPrefab, GameObject.Find("PreviewsHolder").transform);
        SkinPreview.transform.localPosition = new Vector3(Number * 30, 0, 0);
    }

    private void SetShipInformation()
    {
        PilotName.text = Ship.PilotInfo.PilotName;

        string upgradesListText = "";
        foreach (GenericUpgrade upgrade in Ship.UpgradeBar.GetUpgradesAll())
        {
            upgradesListText += upgrade.UpgradeInfo.Name + "\n";
        }
        UpgradesList.text = upgradesListText;
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

    public void NextSkin()
    {
        SkinIndex = (SkinIndex == AvailableSkins.Count - 1) ? 0 : SkinIndex + 1;
        ApplySkinChangeSkin(SkinIndex);
    }

    public void PreviousSkin()
    {
        SkinIndex = (SkinIndex == 0) ? AvailableSkins.Count - 1 : SkinIndex - 1;
        ApplySkinChangeSkin(SkinIndex);
    }

    private int GetCurrentSkinIndex()
    {
        string skinName = (Ship.PilotInfo as PilotCardInfo25).SkinName ?? Ship.ModelInfo.SkinName;
        Texture currentTexture = AvailableSkins.Find(n => n.name == skinName);
        //return index if skin is found, otherwise default to 0
        return (currentTexture != null) ? AvailableSkins.IndexOf(currentTexture) : 0;
    }

    private void ApplySkinChangeSkin(int index)
    {
        string newSkinName = AvailableSkins[index].name;
        (Ship.PilotInfo as PilotCardInfo25).SkinName = newSkinName;
        Ship.SetShipSkin(ShipModel.transform, AvailableSkins[index]);
        SkinName.text = newSkinName;
    }
}
