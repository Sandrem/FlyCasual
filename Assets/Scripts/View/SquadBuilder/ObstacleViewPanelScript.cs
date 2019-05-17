using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleViewPanelScript : MonoBehaviour
{
    public string ObstacleId;
    public Text ObstacleName;
    public RawImage ObstacleRender;
    private GameObject ObstaclePreview;
    private int Number;
    private GameObject ObstacleModel;

    private List<Texture> AvailableSkins;

    // Start is called before the first frame update
    void Start()
    {
        SetObstacleInformation();
        CreatePreviewGroup();
        CreateObstacleModel();
        PrepareCamera();
    }

    private void PrepareCamera()
    {
        RenderTexture cameraTexurePrefab = Resources.Load<RenderTexture>("Textures/CameraSkinPreviewTexture");
        RenderTexture cameraTexure = Instantiate<RenderTexture>(cameraTexurePrefab);
        Camera camera = ObstaclePreview.GetComponentInChildren<Camera>();
        camera.targetTexture = cameraTexure;
        ObstacleRender.texture = cameraTexure;
    }

    private void CreateObstacleModel()
    {
        GameObject obstacleHolder = ObstaclePreview.transform.Find("ObstacleHolder").gameObject;
        GameObject obstacleModelPrefab = Resources.Load<GameObject>("Prefabs/Obstacles/Asteroids/" + ObstacleId);
        ObstacleModel = GameObject.Instantiate<GameObject>(obstacleModelPrefab, obstacleHolder.transform);
    }

    private void CreatePreviewGroup()
    {
        GameObject obstaclePreviewGroupPrefab = Resources.Load<GameObject>("Prefabs/SquadBuilder/ObstaclePreviewGroup");
        ObstaclePreview = GameObject.Instantiate<GameObject>(obstaclePreviewGroupPrefab, GameObject.Find("PreviewsHolder").transform);
        ObstaclePreview.transform.localPosition = new Vector3(Number * 30, 0, 0);
    }

    private void SetObstacleInformation()
    {
        ObstacleName.text = ObstacleId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(string obstacleId, int number)
    {
        ObstacleId = obstacleId;
        Number = number;
    }
}
