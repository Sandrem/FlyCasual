using BoardTools;
using Bombs;
using Movement;
using System.Collections.Generic;
using System.Linq;
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
            HostShip.OnGetAvailableBombLaunchTemplates += TrajectorySimulatorTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombLaunchTemplates -= TrajectorySimulatorTemplate;
        }

        protected virtual void TrajectorySimulatorTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Bomb) return;

            ManeuverTemplate newTemplate = new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed5);

            if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
            {
                availableTemplates.Add(newTemplate);
            }
        }
    }
}

namespace SubPhases
{

    public class BombLaunchPlanningSubPhase : GenericSubPhase
    {
        private List<GameObject> BombObjects = new List<GameObject>();
        List<ManeuverTemplate> AvailableBombLaunchTemplates = new List<ManeuverTemplate>();
        public ManeuverTemplate SelectedBombLaunchHelper;

        public override void Start()
        {
            Name = "Bomb launch planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBombLaunchPlanning();
        }

        public void StartBombLaunchPlanning()
        {
            Roster.SetRaycastTargets(false);

            ShowBombLaunchHelper();
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
            GenerateAllowedBombLaunchDirections();

            if (AvailableBombLaunchTemplates.Count == 1)
            {
                ShowBombAndLaunchTemplate(AvailableBombLaunchTemplates.First());

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
                Name = "Select template to launch the bomb",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, WaitAndSelectBombPosition);
        }

        private void StartSelectTemplateDecision(object sender, System.EventArgs e)
        {
            SelectBombLaunchTemplateDecisionSubPhase selectBoostTemplateDecisionSubPhase = (SelectBombLaunchTemplateDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select template to launch the bomb",
                typeof(SelectBombLaunchTemplateDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectBoostTemplateDecisionSubPhase.ShowSkipButton = false;

            foreach (var bombDropTemplate in AvailableBombLaunchTemplates)
            {
                selectBoostTemplateDecisionSubPhase.AddDecision(
                    bombDropTemplate.Name,
                    delegate { SelectTemplate(bombDropTemplate); },
                    isCentered: (bombDropTemplate.Direction == Movement.ManeuverDirection.Forward)
                );
            }

            selectBoostTemplateDecisionSubPhase.InfoText = "Select template to launch the bomb";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = selectBoostTemplateDecisionSubPhase.GetDecisions().First().Name;

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
        }

        private void SelectTemplate(ManeuverTemplate selectedTemplate)
        {
            ShowBombAndLaunchTemplate(selectedTemplate);
            DecisionSubPhase.ConfirmDecision();
        }

        private class SelectBombLaunchTemplateDecisionSubPhase : DecisionSubPhase { }

        private void GenerateAllowedBombLaunchDirections()
        {
            List<ManeuverTemplate> allowedTemplates = Selection.ThisShip.GetAvailableBombLaunchTemplates(BombsManager.CurrentBomb);

            foreach (ManeuverTemplate bombLaunchTemplate in allowedTemplates)
            {
                AvailableBombLaunchTemplates.Add(bombLaunchTemplate);
            }
        }

        private void ShowBombAndLaunchTemplate(ManeuverTemplate bombDropTemplate)
        {
            bombDropTemplate.ApplyTemplate(Selection.ThisShip, Selection.ThisShip.GetPosition(), Direction.Top);

            Vector3 bombPosition = bombDropTemplate.GetFinalPosition();
            Quaternion bombRotation = bombDropTemplate.GetFinalRotation();
            CreateBombObject(bombPosition, bombRotation);
            BombObjects[0].transform.position = bombPosition;

            SelectedBombLaunchHelper = bombDropTemplate;
        }

        private void WaitAndSelectBombPosition()
        {
            GameManagerScript.Wait(1f, SelectBombPosition);
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
            SelectedBombLaunchHelper.DestroyTemplate();
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