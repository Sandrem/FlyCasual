using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using GameModes;
using System.Linq;

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
            if (Selection.ThisShip.Owner.GetType() == typeof(Players.HotacAiPlayer))
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
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

        public bool IsTractorBeamBoost = false;

        public override void Start()
        {
            Name = "Boost planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostPlanning();
        }

        public void InitializeRendering()
        {
            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            ShipStand = MonoBehaviour.Instantiate(prefab, TheShip.GetPosition(), TheShip.GetRotation(), BoardManager.GetBoard());
            ShipStand.transform.position = new Vector3(ShipStand.transform.position.x, 0, ShipStand.transform.position.z);
            foreach (Renderer render in ShipStand.transform.Find("ShipBase").GetComponentsInChildren<Renderer>())
            {
                render.enabled = false;
            }
            ShipStand.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase = ShipStand.GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorBase.TheShip = TheShip;
            Roster.SetRaycastTargets(false);
        }

        public void StartBoostPlanning()
        {
            foreach (Actions.BoostTemplates boostHelper in TheShip.GetAvailableBoostTemplates())
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

            InitializeRendering();

            AskSelectTemplate();
        }

        private void AskSelectTemplate()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select template for Boost",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = TheShip.Owner.PlayerNo,
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

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = TheShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
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

        public void TryPerformBoost()
        {
            GameMode.CurrentGameMode.TryConfirmBoostPosition(SelectedBoostHelper);
        }

        private void ShowBoosterHelper()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(true);

            Transform newBase = TheShip.GetBoosterHelper().Find(SelectedBoostHelper + "/Finisher/BasePosition");
            ShipStand.transform.position = new Vector3(newBase.position.x, 0, newBase.position.z);
            ShipStand.transform.rotation = newBase.rotation;

            obstaclesStayDetectorMovementTemplate = TheShip.GetBoosterHelper().Find(SelectedBoostHelper).GetComponentInChildren<ObstaclesStayDetectorForced>();
            obstaclesStayDetectorMovementTemplate.TheShip = TheShip;
        }

        public void StartBoostExecution()
        {
            BoostExecutionSubPhase execution = (BoostExecutionSubPhase) Phases.StartTemporarySubPhaseNew(
                "Boost execution",
                typeof(BoostExecutionSubPhase),
                CallBack
            );
            execution.TheShip = TheShip;
            execution.IsTractorBeamBoost = IsTractorBeamBoost;
            execution.Start();
        }

        public void CancelBoost()
        {
            TheShip.IsLandedOnObstacle = false;

            TheShip.RemoveAlreadyExecutedAction(typeof(ActionsList.BoostAction));
            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void HidePlanningTemplates()
        {
            TheShip.GetBoosterHelper().Find(SelectedBoostHelper).gameObject.SetActive(false);
            MonoBehaviour.Destroy(ShipStand);

            Roster.SetRaycastTargets(true);
        }

        public void TryConfirmBoostPositionNetwork(string selectedBoostHelper)
        {
            TryConfirmBoostPosition();
        }

        public void TryConfirmBoostPosition(System.Action<bool> canBoostCallback = null)
        {
            ShowBoosterHelper();

            obstaclesStayDetectorBase.ReCheckCollisionsStart();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsStart();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(() => UpdateColisionDetection(canBoostCallback));
        }

        private bool UpdateColisionDetection(System.Action<bool> canBoostCallback = null)
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults(canBoostCallback);
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults(System.Action<bool> canBoostCallback = null)
        {
            obstaclesStayDetectorBase.ReCheckCollisionsFinish();
            obstaclesStayDetectorMovementTemplate.ReCheckCollisionsFinish();
            HidePlanningTemplates();

            if (canBoostCallback != null)
            {
                canBoostCallback(IsBoostAllowed(true));
                return;
            }

            if (IsBoostAllowed())
            {
                CheckMines();
                TheShip.IsLandedOnObstacle = obstaclesStayDetectorBase.OverlapsAsteroidNow;
                TheShip.ObstaclesHit = new List<Collider>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                obstaclesStayDetectorMovementTemplate.OverlappedAsteroidsNow
                    .Where((a) => !TheShip.ObstaclesHit.Contains(a)).ToList()
                    .ForEach(TheShip.ObstaclesHit.Add);
                GameMode.CurrentGameMode.StartBoostExecution();
            }
            else
            {
                GameMode.CurrentGameMode.CancelBoost();
            }
        }

        private void CheckMines()
        {
            foreach (var mineCollider in obstaclesStayDetectorMovementTemplate.OverlapedMinesNow)
            {
                GameObject mineObject = mineCollider.transform.parent.gameObject;
                if (!TheShip.MinesHit.Contains(mineObject)) TheShip.MinesHit.Add(mineObject);
            }
        }

        private bool IsBoostAllowed(bool quiet = false)
        {
            bool allow = true;

            if (obstaclesStayDetectorBase.OverlapsShipNow || obstaclesStayDetectorMovementTemplate.OverlapsShipNow)
            {
                if (!quiet) Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if (!TheShip.IsIgnoreObstacles && !IsTractorBeamBoost
                && (obstaclesStayDetectorBase.OverlapsAsteroidNow || obstaclesStayDetectorMovementTemplate.OverlapsAsteroidNow))
            {
                if (!quiet) Messages.ShowError("Cannot overlap asteroid");
                allow = false;
            }
            else if (obstaclesStayDetectorBase.OffTheBoardNow || obstaclesStayDetectorMovementTemplate.OffTheBoardNow)
            {
                if (!quiet) Messages.ShowError("Cannot leave the battlefield");
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
        public bool IsTractorBeamBoost;

        public override void Start()
        {
            Name = "Boost execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBoostExecution();
        }

        private void StartBoostExecution()
        {
            Rules.Collision.ClearBumps(TheShip);

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

            boostMovement.TheShip = TheShip;

            MovementTemplates.ApplyMovementRuler(TheShip, boostMovement);

            boostMovement.Perform();
            if (!IsTractorBeamBoost) Sounds.PlayFly(TheShip);
        }

        public void FinishBoost()
        {
            GameMode.CurrentGameMode.FinishBoost();
        }

        public override void Next()
        {
            TheShip.FinishPosition(FinishBoostAnimation);
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
