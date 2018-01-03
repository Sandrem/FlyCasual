using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using GameModes;

namespace ActionsList
{

    public class BoostAction : GenericAction
    {
        public BoostAction()
        {
            Name = "Boost";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/BoostAction.png";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            Phases.StartTemporarySubPhaseOld(
                "Boost",
                typeof(SubPhases.BoostPlanningSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
        }

    }

}

namespace SubPhases
{

    public class BoostPlanningSubPhase : GenericSubPhase
    {
        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public bool inReposition;

        private int updatesCount = 0;

        List<string> AvailableBoostDirections = new List<string>();
        public string SelectedBoostHelper;

        public override void Start()
        {
            Name = "Boost planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostPlanning();
        }

        public void StartBoostPlanning()
        {
            foreach (Actions.BoostTemplates boostHelper in Selection.ThisShip.GetAvailableBoostTemplates())
            {
                switch (boostHelper)
                {
                    case Actions.BoostTemplates.Straight1:
                        AvailableBoostDirections.Add("Straight 1");
                        break;
                    case Actions.BoostTemplates.RightBank1:
                        AvailableBoostDirections.Add("Bank 1 Right");
                        break;
                    case Actions.BoostTemplates.LeftBank1:
                        AvailableBoostDirections.Add("Bank 1 Left");
                        break;
                    case Actions.BoostTemplates.RightTurn1:
                        AvailableBoostDirections.Add("Turn 1 Right");
                        break;
                    case Actions.BoostTemplates.LeftTurn1:
                        AvailableBoostDirections.Add("Turn 1 Left");
                        break;
                    default:
                        AvailableBoostDirections.Add("Straight 1");
                        break;
                }
            }

            GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.position = new Vector3(ShipStand.transform.position.x, 0, ShipStand.transform.position.z);
            foreach (Renderer render in ShipStand.transform.Find("ShipBase").GetComponentsInChildren<Renderer>())
            {
                render.enabled = false;
            }
            ShipStand.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();
            Roster.SetRaycastTargets(false);

            AskSelectTemplate();
        }

        private void AskSelectTemplate()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select template for Boost",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = StartSelectTemplateDecision
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, SelectTemplateDecisionIsTaken);
        }

        private void StartSelectTemplateDecision(object sender, System.EventArgs e)
        {
            SelectBoostTemplateDecisionSubPhase selectBoostTemplateDecisionSubPhase = (SelectBoostTemplateDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select boost template decision",
                typeof(SelectBoostTemplateDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (var boostDirection in AvailableBoostDirections)
            {
                selectBoostTemplateDecisionSubPhase.AddDecision(
                    boostDirection,
                    delegate { SelectTemplate(boostDirection); }
                );
            }

            selectBoostTemplateDecisionSubPhase.InfoText = "Select boost direction";

            selectBoostTemplateDecisionSubPhase.DefaultDecision = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
            UI.ShowSkipButton();
        }

        private class SelectBoostTemplateDecisionSubPhase : DecisionSubPhase { }

        private void SelectTemplate(string templateName)
        {
            SelectedBoostHelper = templateName;
            DecisionSubPhase.ConfirmDecision();
        }

        private void SelectTemplateDecisionIsTaken()
        {
            if (SelectedBoostHelper != null)
            {
                TryPerformBoost();
            }
            else
            {
                CancelBoost();
            }
        }

        private void TryPerformBoost()
        {
            GameMode.CurrentGameMode.TryConfirmBoostPosition(SelectedBoostHelper);
        }

        private void ShowBoosterHelper()
        {
            Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(true);

            Transform newBase = Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper + "/Finisher/BasePosition");
            ShipStand.transform.position = new Vector3(newBase.position.x, 0, newBase.position.z);
            ShipStand.transform.rotation = newBase.rotation;

            obstaclesStayDetectorMovementTemplate = Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper).GetComponentInChildren<ObstaclesStayDetectorForced>();
        }

        public void StartBoostExecution(Ship.GenericShip ship)
        {
            Phases.StartTemporarySubPhaseOld(
                "Boost execution",
                typeof(BoostExecutionSubPhase),
                CallBack
            );
        }

        public void CancelBoost()
        {
            Selection.ThisShip.IsLandedOnObstacle = false;
            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void HidePlanningTemplates()
        {
            Selection.ThisShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(ShipStand);

            Roster.SetRaycastTargets(true);
        }

        public void TryConfirmBoostPositionNetwork(string selectedBoostHelper)
        {
            TryConfirmBoostPosition();
        }

        public void TryConfirmBoostPosition()
        {
            ShowBoosterHelper();

            obstaclesStayDetectorBase.ReCheckCollisionsStart();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        private bool UpdateColisionDetection()
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults();
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults()
        {
            obstaclesStayDetectorBase.ReCheckCollisionsFinish();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsFinish();

            if (IsBoostAllowed())
            {
                CheckMines();
                Selection.ThisShip.IsLandedOnObstacle = obstaclesStayDetectorBase.OverlapsAsteroidNow;
                GameMode.CurrentGameMode.StartBoostExecution(Selection.ThisShip);
            }
            else
            {
                GameMode.CurrentGameMode.CancelBoost();
            }

            HidePlanningTemplates();
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!Selection.ThisShip.MinesHit.Contains(mineObject)) Selection.ThisShip.MinesHit.Add(mineObject);
            }
        }

        private bool IsBoostAllowed()
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow || obstaclesStayDetectorMovementTemplate.OverlapsShipNow)
            {
                Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if ((!Selection.ThisShip.IsIgnoreObstacles) && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
            {
                Messages.ShowError("Cannot overlap asteroid");
                allow = false;
            }
            else if (obstaclesStayDetectorBase.OffTheBoardNow || obstaclesStayDetectorMovementTemplate.OffTheBoardNow)
            {
                Messages.ShowError("Cannot leave the battlefield");
                allow = false;
            }

            return allow;
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
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

    public class BoostExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Boost execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostExecution();
        }

        private void StartBoostExecution()
        {
            Movement.GenericMovement boostMovement;
            switch ((PreviousSubPhase as BoostPlanningSubPhase).SelectedBoostHelper)
            {
                case "Straight 1":
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.None);
                    break;
                case "Bank 1 Left":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Left, Movement.ManeuverBearing.Bank, Movement.ManeuverColor.None);
                    break;
                case "Bank 1 Right":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Right, Movement.ManeuverBearing.Bank, Movement.ManeuverColor.None);
                    break;
                case "Turn 1 Right":
                    boostMovement = new Movement.TurnBoost(1, Movement.ManeuverDirection.Right, Movement.ManeuverBearing.Turn, Movement.ManeuverColor.None);
                    break;
                case "Turn 1 Left":
                    boostMovement = new Movement.TurnBoost(1, Movement.ManeuverDirection.Left, Movement.ManeuverBearing.Turn, Movement.ManeuverColor.None);
                    break;
                default:
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.None);
                    break;
            }

            MovementTemplates.ApplyMovementRuler(Selection.ThisShip, boostMovement);

            boostMovement.Perform();
            Sounds.PlayFly();
        }

        public void FinishBoost()
        {
            GameMode.CurrentGameMode.FinishBoost();
        }

        public override void Next()
        {
            Selection.ThisShip.FinishPosition(FinishBoostAnimation);
        }

        private void FinishBoostAnimation()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

}
