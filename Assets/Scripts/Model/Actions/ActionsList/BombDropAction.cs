using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
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

            BombsManager.CurrentBomb = Source as Upgrade.GenericBomb;
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(SubPhases.BombDropPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override bool IsActionAvailable()
        {
            return !Selection.ThisShip.IsBombAlreadyDropped;
        }
    }

}

namespace SubPhases
{

    public class BombDropPlanningSubPhase : GenericSubPhase
    {
        Dictionary<string, Vector3> AvailableBombDropDirections = new Dictionary<string, Vector3>();
        public string SelectedBombDropHelper;
        private List<GameObject> BombObjects = new List<GameObject>();
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
            GenerateAllowedBombDropDirections();

            if (AvailableBombDropDirections.Count == 1)
            {
                ShowBombAndDropHelper(AvailableBombDropDirections.First().Key);

                WaitAndSelectBombPosition();
            }
            else
            {
                AskSelectTemplate();
            }
        }

        private void AskSelectTemplate()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select template to drop the bomb",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, WaitAndSelectBombPosition);
        }

        private void StartSelectTemplateDecision(object sender, System.EventArgs e)
        {
            SelectBombDropTemplateDecisionSubPhase selectBoostTemplateDecisionSubPhase = (SelectBombDropTemplateDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select template to drop the bomb",
                typeof(SelectBombDropTemplateDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (var bombDropTemplate in AvailableBombDropDirections)
            {
                selectBoostTemplateDecisionSubPhase.AddDecision(
                    bombDropTemplate.Key,
                    delegate { SelectTemplate(bombDropTemplate.Key); }
                );
            }

            selectBoostTemplateDecisionSubPhase.InfoText = "Select template to drop the bomb";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
            UI.ShowSkipButton();
        }

        private void SelectTemplate(string selectedTemplate)
        {
            ShowBombAndDropHelper(selectedTemplate);
            DecisionSubPhase.ConfirmDecision();
        }

        private class SelectBombDropTemplateDecisionSubPhase : DecisionSubPhase { }

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

        private void GenerateAllowedBombDropDirections()
        {
            List<BombDropTemplates> allowedTemplates = Selection.ThisShip.GetAvailableBombDropTemplates();

            foreach (Transform bombDropHelper in Selection.ThisShip.GetBombDropHelper())
            {
                if (allowedTemplates.Contains((BombDropTemplates)System.Enum.Parse(typeof(BombDropTemplates), bombDropHelper.name.Replace(" ", "_"))))
                {
                    AvailableBombDropDirections.Add(bombDropHelper.name, bombDropHelper.Find("Finisher").position);
                }
            }
        }

        private void ShowBombAndDropHelper(string name)
        {
            CreateBombObject(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation());

            if (!string.IsNullOrEmpty(SelectedBombDropHelper))
            {
                Selection.ThisShip.GetBombDropHelper().Find(SelectedBombDropHelper).gameObject.SetActive(false);
            }
            Selection.ThisShip.GetBombDropHelper().Find(name).gameObject.SetActive(true);

            Transform newBase = Selection.ThisShip.GetBombDropHelper().Find(name + "/Finisher/BasePosition");

            for (int i = 0; i < BombObjects.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        BombObjects[i].transform.position = new Vector3(
                            newBase.position.x,
                            0,
                            newBase.position.z
                        );
                        break;
                    case 1:
                        BombObjects[i].transform.position = new Vector3(
                            newBase.position.x,
                            0,
                            newBase.position.z)
                            +
                            newBase.TransformVector(new Vector3(
                                BombsManager.CurrentBomb.bombSideDistanceX,
                                0,
                                BombsManager.CurrentBomb.bombSideDistanceZ
                            )
                        );
                        break;
                    case 2:
                        BombObjects[i].transform.position = new Vector3(
                            newBase.position.x,
                            0,
                            newBase.position.z)
                            +
                            newBase.TransformVector(new Vector3(
                                -BombsManager.CurrentBomb.bombSideDistanceX,
                                0,
                                BombsManager.CurrentBomb.bombSideDistanceZ
                            )
                        );
                        break;
                    default:
                        break;
                }

                BombObjects[i].transform.rotation = newBase.rotation;
            }

            SelectedBombDropHelper = name;
        }

        private void WaitAndSelectBombPosition()
        {
            //Temporary
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1f, SelectBombPosition);
        }

        private void SelectBombPosition()
        {
            HidePlanningTemplates();
            BombDropExecute();
        }

        private void BombDropExecute()
        {
            BombsManager.CurrentBomb.ActivateBombs(BombObjects, FinishAction);
        }

        private void FinishAction()
        {
            Phases.FinishSubPhase(typeof(BombDropPlanningSubPhase));
            CallBack();
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
