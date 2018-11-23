using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using GameModes;
using System.Linq;
using RuleSets;
using Obstacles;
using ActionsList;

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
            if (Selection.ThisShip.Owner.UsesHotacAiRules)
            {
                Phases.CurrentSubPhase.CallBack();
            }
            else
            {
                Phases.CurrentSubPhase.Pause();
                var phase = Phases.StartTemporarySubPhaseNew<SubPhases.BoostPlanningSubPhase>(
                    "Boost",
                    Phases.CurrentSubPhase.CallBack
                );
                phase.HostAction = this;
                phase.Start();
            }
        }

    }

    public class BoostMove
    {
        public string Name { get; private set; }
        public ActionsHolder.BoostTemplates Template;
        public bool IsRed;

        public BoostMove(ActionsHolder.BoostTemplates template, bool isRed = false)
        {
            Template = template;
            IsRed = isRed;

            switch (template)
            {
                case ActionsHolder.BoostTemplates.Straight1:
                    Name = "Straight 1";
                    break;
                case ActionsHolder.BoostTemplates.RightBank1:
                    Name = "Bank 1 Right";
                    break;
                case ActionsHolder.BoostTemplates.LeftBank1:
                    Name = "Bank 1 Left";
                    break;
                case ActionsHolder.BoostTemplates.RightTurn1:
                    Name = "Turn 1 Right";
                    break;
                case ActionsHolder.BoostTemplates.LeftTurn1:
                    Name = "Turn 1 Left";
                    break;
                default:
                    Name = "Straight 1";
                    break;
            }
        }
    }
}

namespace SubPhases
{

    public class BoostPlanningSubPhase : GenericSubPhase
    {
        public GenericAction HostAction;
        public GameObject ShipStand;
        private ObstaclesStayDetectorForced obstaclesStayDetectorBase;
        private ObstaclesStayDetectorForced obstaclesStayDetectorMovementTemplate;

        public bool inReposition;

        private int updatesCount = 0;

        List<BoostMove> AvailableBoostMoves = new List<BoostMove>();
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
            ShipStand = MonoBehaviour.Instantiate(prefab, TheShip.GetPosition(), TheShip.GetRotation(), BoardTools.Board.GetBoard());
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
            AvailableBoostMoves = TheShip.GetAvailableBoostTemplates();

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

            foreach (var move in AvailableBoostMoves)
            {
                selectBoostTemplateDecisionSubPhase.AddDecision(
                    move.Name,
                    delegate { SelectTemplate(move); },
                    isRed: move.IsRed,
                    isCentered: move.Template == ActionsHolder.BoostTemplates.Straight1
                );
            }

            selectBoostTemplateDecisionSubPhase.InfoText = "Select boost direction";

            selectBoostTemplateDecisionSubPhase.DefaultDecisionName = "Straight 1";

            selectBoostTemplateDecisionSubPhase.RequiredPlayer = TheShip.Owner.PlayerNo;

            selectBoostTemplateDecisionSubPhase.Start();
        }

        private class SelectBoostTemplateDecisionSubPhase : DecisionSubPhase { }

        private void SelectTemplate(BoostMove move)
        {
            if (move.IsRed && !HostAction.IsRed)
            {
                HostAction.IsRed = true;
                TheShip.OnActionIsPerformed += ResetActionColor;
            }
                
            SelectedBoostHelper = move.Name;
            DecisionSubPhase.ConfirmDecision();
        }

        private void ResetActionColor(GenericAction action)
        {
            action.Host.OnActionIsPerformed -= ResetActionColor;
            HostAction.IsRed = false;
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

            MonoBehaviour.Destroy(ShipStand);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            Edition.Instance.ActionIsFailed(TheShip, typeof(BoostAction));
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
                TheShip.LandedOnObstacles = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
                TheShip.ObstaclesHit = new List<GenericObstacle>(obstaclesStayDetectorBase.OverlappedAsteroidsNow);
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

            if (obstaclesStayDetectorBase.OverlapsShipNow)
            {
                if (!quiet) Messages.ShowError("Cannot overlap another ship");
                allow = false;
            }
            else if (!TheShip.IsIgnoreObstacles && !TheShip.IsIgnoreObstaclesDuringBoost && !IsTractorBeamBoost
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
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.MovementComplexity.None);
                    break;
                case "Bank 1 Left":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Left, Movement.ManeuverBearing.Bank, Movement.MovementComplexity.None);
                    break;
                case "Bank 1 Right":
                    boostMovement = new Movement.BankBoost(1, Movement.ManeuverDirection.Right, Movement.ManeuverBearing.Bank, Movement.MovementComplexity.None);
                    break;
                case "Turn 1 Right":
                    boostMovement = new Movement.TurnBoost(1, Movement.ManeuverDirection.Right, Movement.ManeuverBearing.Turn, Movement.MovementComplexity.None);
                    break;
                case "Turn 1 Left":
                    boostMovement = new Movement.TurnBoost(1, Movement.ManeuverDirection.Left, Movement.ManeuverBearing.Turn, Movement.MovementComplexity.None);
                    break;
                default:
                    boostMovement = new Movement.StraightBoost(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.MovementComplexity.None);
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
