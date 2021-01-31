using Obstacles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public class SquadBuilderObstaclesView
    {
        private GenericObstacle CurrentObstacle { get; set; }

        public void SetDefaultObstacles()
        {
            Global.SquadBuilder.CurrentSquad.SetDefaultObstacles();
            ShowChosenObstaclesPanel();
        }

        public void ShowChosenObstaclesPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/ObstacleViewPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/ChosenObstaclesPanel/Content/ObstaclesPanel").gameObject;

            SquadBuilderView.DestroyChildren(contentGO.transform);
            SquadBuilderView.DestroyChildren(GameObject.Find("PreviewsHolder").transform);

            for (int i = 0; i < 3; i++)
            {
                GameObject newObstacleViewPanel = GameObject.Instantiate(prefab, contentGO.transform);
                GenericObstacle obstacle = Global.SquadBuilder.CurrentSquad.ChosenObstacles[i];
                newObstacleViewPanel.GetComponent<ObstacleViewPanelScript>().Initialize(
                    obstacle,
                    i + 1,
                    delegate {
                        CurrentObstacle = obstacle;
                        MainMenu.CurrentMainMenu.ChangePanel("BrowseObstaclesPanel");
                    }
                );
            }

            MainMenu.ScalePanel(contentGO.transform);
        }

        public void ShowBrowseObstaclesPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/ObstacleViewPanelSmall";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/BrowseObstaclesPanel/Content").gameObject;

            SquadBuilderView.DestroyChildren(contentGO.transform);
            SquadBuilderView.DestroyChildren(GameObject.Find("PreviewsHolder").transform);

            for (int i = 0; i < ObstaclesManager.Instance.AllPossibleObstacles.Count; i++)
            {
                GameObject newObstacleViewPanel = GameObject.Instantiate(prefab, contentGO.transform);
                GenericObstacle obstacle = ObstaclesManager.Instance.AllPossibleObstacles[i];
                newObstacleViewPanel.GetComponent<ObstacleViewPanelScript>().Initialize(
                    obstacle,
                    i,
                    delegate {
                        int index = Global.SquadBuilder.CurrentSquad.ChosenObstacles.IndexOf(CurrentObstacle);
                        Global.SquadBuilder.CurrentSquad.ChosenObstacles[index] = obstacle;
                        MainMenu.CurrentMainMenu.ChangePanel("ChosenObstaclesPanel");
                    }
                );
                if (Global.SquadBuilder.CurrentSquad.ChosenObstacles.Contains(obstacle) && obstacle != CurrentObstacle)
                {
                    newObstacleViewPanel.GetComponent<Button>().interactable = false;
                }
            }

        }
    }
}
