using Obstacles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleViewPanelScript : MonoBehaviour
{
    public GenericObstacle Obstacle;
    public Text ObstacleName;
    public RawImage ObstacleRender;
    private GameObject ObstaclePreview;
    private int Number;
    private GameObject ObstacleModel;
    private Action OnClick;

    // Start is called before the first frame update
    void Start()
    {
        SetObstacleInformation();
        CreatePreviewGroup();
        CreateObstacleModel();
        PrepareCamera();
        PrepareButton();
    }

    private void PrepareButton()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
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
        GameObject obstacleModelPrefab = Resources.Load<GameObject>(string.Format("Prefabs/Obstacles/{0}/{1}", Obstacle.GetTypeName, Obstacle.Name));
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
        ObstacleName.text = Obstacle.Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(GenericObstacle obstacle, int number, Action onClick)
    {
        Obstacle = obstacle;
        Number = number;
        OnClick = onClick;
    }
}
