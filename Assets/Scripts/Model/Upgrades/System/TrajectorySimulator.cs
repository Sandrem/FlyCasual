using Upgrade;
using Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using BoardTools;
using RuleSets;

namespace UpgradesList
{
    public class TrajectorySimulator : GenericUpgrade, ISecondEditionUpgrade
    {
        public TrajectorySimulator() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Trajectory Simulator";
            Cost = 1;

            UpgradeAbilities.Add (new TrajectorySimulatorAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;
        }
    }
}

namespace Abilities
{
    public class TrajectorySimulatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanLaunchBombs = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanLaunchBombs = false;
        }
    }
}

namespace SubPhases
{

    public class BombLaunchPlanningSubPhase : GenericSubPhase
    {
        private List<GameObject> BombObjects = new List<GameObject>();

        public override void Start()
        {
            Name = "Bomb launch planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBombLaunchPlanning();
        }

        public void StartBombLaunchPlanning()
        {
            CreateBombObject(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation());

            Roster.SetRaycastTargets(false);

            ShowBombLaunchHelper();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(0.5f, SelectBombPosition);
        }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GameObject prefab = (GameObject)Resources.Load(BombsManager.CurrentBomb.bombPrefabPath, typeof(GameObject));
            BombObjects.Add(MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, BoardTools.Board.GetBoard()));

            if (!string.IsNullOrEmpty(BombsManager.CurrentBomb.bombSidePrefabPath))
            {
                GameObject prefabSide = (GameObject)Resources.Load(BombsManager.CurrentBomb.bombSidePrefabPath, typeof(GameObject));
                BombObjects.Add(MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, BoardTools.Board.GetBoard()));
                BombObjects.Add(MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, BoardTools.Board.GetBoard()));
            }
        }

        private void ShowBombLaunchHelper()
        {
            Selection.ThisShip.GetBombLaunchHelper().Find("Straight5").gameObject.SetActive(true);

            Transform newBase = Selection.ThisShip.GetBombLaunchHelper().Find("Straight5/Finisher/BasePosition");

            // Cluster Mines cannot be launched - only single model is handled
            BombObjects[0].transform.position = new Vector3(
                newBase.position.x,
                0,
                newBase.position.z
            );

            BombObjects[0].transform.rotation = newBase.rotation;
        }

        private void SelectBombPosition()
        {
            HidePlanningTemplates();
            BombLaunchExecute();
        }

        private void BombLaunchExecute()
        {
            BombsManager.CurrentBomb.ActivateBombs(BombObjects, FinishAction);
        }

        private void FinishAction()
        {
            Phases.FinishSubPhase(typeof(BombLaunchPlanningSubPhase));
            CallBack();
        }

        private void HidePlanningTemplates()
        {
            Selection.ThisShip.GetBombLaunchHelper().Find("Straight5").gameObject.SetActive(false);
            Roster.SetRaycastTargets(true);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

}
