using Bombs;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TrajectorySimulator : GenericUpgrade
    {
        public TrajectorySimulator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trajectory Simulator",
                UpgradeType.System,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.TrajectorySimulatorAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class TrajectorySimulatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 5;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 0;
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

            GameManagerScript.Wait(0.5f, SelectBombPosition);
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
            string bombLaunchHelperName = "Straight" + Selection.ThisShip.CanLaunchBombsWithTemplate;
            Selection.ThisShip.GetBombLaunchHelper().Find(bombLaunchHelperName).gameObject.SetActive(true);
            Transform newBase = Selection.ThisShip.GetBombLaunchHelper().Find(bombLaunchHelperName + "/Finisher/BasePosition");

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
            string bombLaunchHelperName = "Straight" + Selection.ThisShip.CanLaunchBombsWithTemplate;
            Selection.ThisShip.GetBombLaunchHelper().Find(bombLaunchHelperName).gameObject.SetActive(false);
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