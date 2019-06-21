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
            Name = DiceModificationName = "Drop Bomb";
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
        List<ManeuverTemplate> AvailableBombDropTemplates = new List<ManeuverTemplate>();
        public ManeuverTemplate SelectedBombDropHelper;
        private List<GenericDeviceGameObject> BombObjects = new List<GenericDeviceGameObject>();
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
            GenerateAllowedBombDropTemplates();

            if (AvailableBombDropTemplates.Count == 1)
            {
                ShowBombAndDropTemplate(AvailableBombDropTemplates.First());

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

            selectBoostTemplateDecisionSubPhase.ShowSkipButton = false;

            foreach (var bombDropTemplate in AvailableBombDropTemplates)
            {
                selectBoostTemplateDecisionSubPhase.AddDecision(
                    bombDropTemplate.Name,
                    delegate { SelectTemplate(bombDropTemplate); },
                    isCentered: (bombDropTemplate.Direction == Movement.ManeuverDirection.Forward)
                );
            }

            selectBoostTemplateDecisionSubPhase.InfoText = "Select template to drop the bomb";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
        }

        private void SelectTemplate(ManeuverTemplate selectedTemplate)
        {
            ShowBombAndDropTemplate(selectedTemplate);
            DecisionSubPhase.ConfirmDecision();
        }

        private class SelectBombDropTemplateDecisionSubPhase : DecisionSubPhase { }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GenericDeviceGameObject prefab = Resources.Load<GenericDeviceGameObject>(BombsManager.CurrentBomb.bombPrefabPath);
            var device = MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, BoardTools.Board.GetBoard());
            device.Initialize(BombsManager.CurrentBomb);
            BombObjects.Add(device);
            

            if (!string.IsNullOrEmpty(BombsManager.CurrentBomb.bombSidePrefabPath))
            {
                GenericDeviceGameObject prefabSide = Resources.Load<GenericDeviceGameObject>(BombsManager.CurrentBomb.bombSidePrefabPath);
                var extraPiece1 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, BoardTools.Board.GetBoard());
                var extraPiece2 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, BoardTools.Board.GetBoard());
                BombObjects.Add(extraPiece1);
                BombObjects.Add(extraPiece2);
                extraPiece1.Initialize(BombsManager.CurrentBomb);
                extraPiece2.Initialize(BombsManager.CurrentBomb);
            }
        }

        private void GenerateAllowedBombDropTemplates()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBombDropTemplates(BombsManager.CurrentBomb);

            foreach (ManeuverTemplate bombDropTemplate in allowedTemplates)
            {
                AvailableBombDropTemplates.Add(bombDropTemplate);
            }
        }

        private void ShowBombAndDropTemplate(ManeuverTemplate bombDropTemplate)
        {
            bombDropTemplate.ApplyTemplate(Selection.ThisShip, Selection.ThisShip.GetBack(), Direction.Bottom);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();
            CreateBombObject(bombPosition, bombRotation);

            for (int i = 0; i < BombObjects.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        BombObjects[i].transform.position = bombPosition;
                        break;
                    case 1:
                        BombObjects[i].transform.position = bombPosition
                            + BombObjects.First().transform.TransformVector(new Vector3(
                                BombsManager.CurrentBomb.bombSideDistanceX,
                                0,
                                BombsManager.CurrentBomb.bombSideDistanceZ
                            )
                        );
                        break;
                    case 2:
                        BombObjects[i].transform.position = bombPosition
                            + BombObjects.First().transform.TransformVector(new Vector3(
                                -BombsManager.CurrentBomb.bombSideDistanceX,
                                0,
                                BombsManager.CurrentBomb.bombSideDistanceZ
                            )
                        );
                        break;
                    default:
                        break;
                }

                BombObjects[i].transform.rotation = bombRotation;
            }

            SelectedBombDropHelper = bombDropTemplate;
        }

        private void WaitAndSelectBombPosition()
        {
            GameManagerScript.Wait(1f, SelectBombPosition);
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
            SelectedBombDropHelper.DestroyTemplate();
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
