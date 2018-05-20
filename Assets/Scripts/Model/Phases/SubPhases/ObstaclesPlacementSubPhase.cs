using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BoardTools;
using UnityEngine.EventSystems;
using Obstacles;

namespace SubPhases
{

    public class ObstaclesPlacementSubPhase : GenericSubPhase
    {
        public GenericObstacle ChosenObstacle;

        public override void Start()
        {
            Name = "Obstacles Placement";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Board.ToggleObstaclesHolder(true);
            UI.ShowNextButton();
        }

        public override void Next()
        {
            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartSetupPhase);
            (subphase as NotificationSubPhase).TextToShow = "Setup";
            subphase.Start();
        }

        private void StartSetupPhase()
        {
            Phases.CurrentSubPhase = new SetupStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

        public override void NextButton()
        {
            Board.ToggleObstaclesHolder(false);
            Next();
        }

        public override void Update()
        {
            MoveChosenObstacle();
        }

        private void MoveChosenObstacle()
        {
            if (ChosenObstacle == null) return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ChosenObstacle.ObstacleGO.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
            }

            ApplyLimits();
        }

        private void ApplyLimits()
        {

        }

        public override void ProcessClick()
        {
            if (ChosenObstacle == null)
            {
                TryToSelectObstacle();
            }
            else
            {
                TryToPlaceObstacle();
            }
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Asteroid"))
                        {
                            GameObject obstacleGO = hitInfo.transform.parent.gameObject;
                            GenericObstacle clickedObstacle = ObstaclesManager.Instance.GetObstacleByName(obstacleGO.name);
                            
                            if (!clickedObstacle.IsPlaced)
                            {
                                ChosenObstacle = clickedObstacle;
                            }
                        }
                    }
                }
            }
        }

        private void TryToPlaceObstacle()
        {
            ChosenObstacle.IsPlaced = true;
            ChosenObstacle = null;
        }

    }

}
