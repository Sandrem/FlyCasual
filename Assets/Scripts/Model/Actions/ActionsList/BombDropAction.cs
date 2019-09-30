using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using Bombs;
using System.Linq;
using Upgrade;
using Remote;
using System;

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

            BombsManager.CurrentDevice = Source as Upgrade.GenericBomb;
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
                if (BombsManager.CurrentDevice is GenericBomb)
                {
                    ShowBombAndDropTemplate(AvailableBombDropTemplates.First());
                }
                else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
                {
                    ShowRemoteAndDropTemplate(AvailableBombDropTemplates.First());
                }

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

            selectBoostTemplateDecisionSubPhase.DescriptionShort = "Select template to drop the device";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
        }

        private void SelectTemplate(ManeuverTemplate selectedTemplate)
        {
            if (BombsManager.CurrentDevice is GenericBomb)
            {
                ShowBombAndDropTemplate(selectedTemplate);
            }
            else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                ShowRemoteAndDropTemplate(selectedTemplate);
            }
            
            DecisionSubPhase.ConfirmDecision();
        }

        private void ShowRemoteAndDropTemplate(ManeuverTemplate bombDropTemplate)
        {
            bombDropTemplate.ApplyTemplate(Selection.ThisShip, Selection.ThisShip.GetBack(), Direction.Bottom);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();

            // TODO: get type of remote from upgrade
            ShipFactory.SpawnRemove(
                (GenericRemote) Activator.CreateInstance(BombsManager.CurrentDevice.UpgradeInfo.RemoteType, Selection.ThisShip.Owner),
                bombPosition,
                bombRotation
            );

            SelectedBombDropHelper = bombDropTemplate;
        }

        private class SelectBombDropTemplateDecisionSubPhase : DecisionSubPhase { }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GenericBomb bomb = BombsManager.CurrentDevice as GenericBomb;

            GenericDeviceGameObject prefab = Resources.Load<GenericDeviceGameObject>(bomb.bombPrefabPath);
            var device = MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, Board.GetBoard());
            device.Initialize(bomb);
            BombObjects.Add(device);

            if (!string.IsNullOrEmpty(bomb.bombSidePrefabPath))
            {
                GenericDeviceGameObject prefabSide = Resources.Load<GenericDeviceGameObject>(bomb.bombSidePrefabPath);
                var extraPiece1 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                var extraPiece2 = MonoBehaviour.Instantiate(prefabSide, bombPosition, bombRotation, Board.GetBoard());
                BombObjects.Add(extraPiece1);
                BombObjects.Add(extraPiece2);
                extraPiece1.Initialize(bomb);
                extraPiece2.Initialize(bomb);
            }
        }

        private void GenerateAllowedBombDropTemplates()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBombDropTemplates(BombsManager.CurrentDevice);

            foreach (ManeuverTemplate bombDropTemplate in allowedTemplates)
            {
                AvailableBombDropTemplates.Add(bombDropTemplate);
            }
        }

        private void ShowBombAndDropTemplate(ManeuverTemplate bombDropTemplate)
        {
            GenericBomb bomb = BombsManager.CurrentDevice as GenericBomb;

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
                                bomb.bombSideDistanceX,
                                0,
                                bomb.bombSideDistanceZ
                            )
                        );
                        break;
                    case 2:
                        BombObjects[i].transform.position = bombPosition
                            + BombObjects.First().transform.TransformVector(new Vector3(
                                -bomb.bombSideDistanceX,
                                0,
                                bomb.bombSideDistanceZ
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
            DeviceDropExecute();
        }

        private void DeviceDropExecute()
        {
            if (BombsManager.CurrentDevice is GenericBomb)
            {
                (BombsManager.CurrentDevice as GenericBomb).ActivateBombs(BombObjects, FinishAction);
            }
            else if (BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                // TODO: Activate remote
                FinishAction();
            }
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
