using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Bombs;
using System.Linq;

namespace ActionsList
{

    public class BombDropAction : GenericAction
    {
        public BombDropAction()
        {
            Name = EffectName = "Drop Bomb";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();

            BombsManager.CurrentBombToDrop = Source as Upgrade.GenericBomb;
            Phases.StartTemporarySubPhase(
                "Bomb drop planning",
                typeof(SubPhases.BombDropPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }
    }

}

namespace SubPhases
{

    public class BombDropPlanningSubPhase : GenericSubPhase
    {
        Dictionary<string, Vector3> AvailableBombDropDirections = new Dictionary<string, Vector3>();
        public string SelectedBombDropHelper;
        private GameObject BombObject;
        private bool inReposition;

        public override void Start()
        {
            Name = "Bomb drop planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBombDropPlanning();
        }

        public void StartBombDropPlanning()
        {
            List<BombDropTemplates> allowedTemplates = Selection.ThisShip.GetAvailableBombDropTemplates();
            foreach (Transform bombDropHelper in Selection.ThisShip.GetBombDropHelper())
            {
                if (allowedTemplates.Contains((BombDropTemplates)System.Enum.Parse(typeof(BombDropTemplates), bombDropHelper.name)))
                {
                    AvailableBombDropDirections.Add(bombDropHelper.name, bombDropHelper.Find("Finisher").position);
                }
            }

            GameObject prefab = (GameObject)Resources.Load(BombsManager.CurrentBombToDrop.bombPrefabPath, typeof(GameObject));
            BombObject = MonoBehaviour.Instantiate(prefab, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            Roster.SetRaycastTargets(false);

            inReposition = true;
        }

        public override void Update()
        {
            if (inReposition)
            {
                SelectBombDropHelper();
            }
        }

        public override void Pause()
        {

        }

        public override void Resume()
        {

        }

        private void SelectBombDropHelper()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ShowNearestBombDropHelper(GetNearestBombDropHelper(new Vector3(hit.point.x, 0f, hit.point.z)));
            }
        }

        private void ShowNearestBombDropHelper(string name)
        {
            if (SelectedBombDropHelper != name)
            {
                if (!string.IsNullOrEmpty(SelectedBombDropHelper))
                {
                    Selection.ThisShip.GetBombDropHelper().Find(SelectedBombDropHelper).gameObject.SetActive(false);
                }
                Selection.ThisShip.GetBombDropHelper().Find(name).gameObject.SetActive(true);

                Transform newBase = Selection.ThisShip.GetBombDropHelper().Find(name + "/Finisher/BasePosition");
                BombObject.transform.position = newBase.position;
                BombObject.transform.rotation = newBase.rotation;

                SelectedBombDropHelper = name;
            }
        }

        private string GetNearestBombDropHelper(Vector3 point)
        {
            float minDistance = float.MaxValue;
            KeyValuePair<string, Vector3> nearestBombDropHelper = new KeyValuePair<string, Vector3>();

            foreach (var bombDropDirection in AvailableBombDropDirections)
            {
                if (string.IsNullOrEmpty(nearestBombDropHelper.Key))
                {
                    nearestBombDropHelper = bombDropDirection;
                    minDistance = Vector3.Distance(point, bombDropDirection.Value);
                    continue;
                }
                else
                {
                    float currentDistance = Vector3.Distance(point, bombDropDirection.Value);
                    if (currentDistance < minDistance)
                    {
                        nearestBombDropHelper = bombDropDirection;
                        minDistance = currentDistance;
                    }
                }
            }

            return nearestBombDropHelper.Key;
        }

        public override void ProcessClick()
        {
            StopPlanning();
            HidePlanningTemplates();
            BombDropExecute();
        }

        private void BombDropExecute()
        {
            BombsManager.CurrentBombToDrop.ActivateBomb(BombObject);

            Phases.FinishSubPhase(typeof(BombDropPlanningSubPhase));
            CallBack();
        }

        private void StopPlanning()
        {
            inReposition = false;
        }

        private void HidePlanningTemplates()
        {
            Selection.ThisShip.GetBombDropHelper().Find(SelectedBombDropHelper).gameObject.SetActive(false);
            Roster.SetRaycastTargets(true);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            return false;
        }

    }

}
