using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

namespace ActionsList
{

    public class BarrelRollAction : GenericAction
    {
        public BarrelRollAction() {
            Name = "Barrel Roll";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            Phases.StartTemporarySubPhase(
                "Barrel Roll",
                typeof(SubPhases.BarrelRollPlanningSubPhase),
                Phases.CurrentSubPhase.callBack
            );
        }

    }

}

namespace SubPhases
{

    public class BarrelRollPlanningSubPhase : GenericSubPhase
    {
        public GameObject ShipStand;
        public float helperDirection;
        public bool inReposition;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Barrel Roll planning";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollPlanning();
        }

        public void StartBarrelRollPlanning()
        {
            ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), BoardManager.GetBoard());

            ShipStand.transform.Find("ShipStandTemplate").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;

            Roster.SetRaycastTargets(false);
            inReposition = true;
            MovementTemplates.CurrentTemplate = MovementTemplates.GetMovement1Ruler();
            MovementTemplates.SaveCurrentMovementRulerPosition();
            MovementTemplates.CurrentTemplate.position = Selection.ThisShip.TransformPoint(new Vector3(0.5f, 0, -0.25f));
        }

        public override void Update()
        {
            if (inReposition)
            {
                PerfromDrag();
            }
        }

        public override void Pause()
        {
            inReposition = false;
        }

        public override void Resume()
        {
            inReposition = true;
        }

        private void PerfromDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (ShipStand != null)
                {
                    ShipStand.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    ApplyBarrelRollRepositionLimits();
                }
            }
        }

        private void ApplyBarrelRollRepositionLimits()
        {
            Vector3 newPosition = Selection.ThisShip.InverseTransformPoint(ShipStand.transform.position);
            Vector3 fixedPositionRel = newPosition;

            if (newPosition.z > 0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, 0.5f);
            }

            if (newPosition.z < -0.5f)
            {
                fixedPositionRel = new Vector3(fixedPositionRel.x, fixedPositionRel.y, -0.5f);
            }

            if (newPosition.x > 0f)
            {
                fixedPositionRel = new Vector3(2, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = 1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, 180, 0);
            }

            if (newPosition.x < 0f)
            {
                fixedPositionRel = new Vector3(-2, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = -1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles;
            }

            Vector3 helperPositionRel = Selection.ThisShip.InverseTransformPoint(MovementTemplates.CurrentTemplate.position);
            helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, helperPositionRel.z);

            if (helperPositionRel.z + 0.25f > fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.25f);
            }

            if (helperPositionRel.z + 0.75f < fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.75f);
            }

            Vector3 helperPositionAbs = Selection.ThisShip.TransformPoint(helperPositionRel);
            MovementTemplates.CurrentTemplate.position = helperPositionAbs;

            Vector3 fixedPositionAbs = Selection.ThisShip.TransformPoint(fixedPositionRel);
            ShipStand.transform.position = fixedPositionAbs;
        }

        public override void ProcessClick()
        {
            TryConfirmPosition(Selection.ThisShip);
        }

        private bool TryConfirmPosition(Ship.GenericShip ship)
        {
            StopDrag();

            bool result = false;

            result = TryConfirmBarrelRollPosition(ship);

            if (result)
            {
                StartBarrelRollExecution(ship);
            }
            else
            {
                CancelBarrelRoll();
            }

            return result;
        }

        private void StartBarrelRollExecution(Ship.GenericShip ship)
        {
            Pause();

            Selection.ThisShip.ToggleShipStandAndPeg(false);
            MovementTemplates.CurrentTemplate.gameObject.SetActive(false);

            Phases.StartTemporarySubPhase(
                "Barrel Roll execution",
                typeof(BarrelRollExecutionSubPhase),
                callBack
            );
        }

        private void CancelBarrelRoll()
        {
            Selection.ThisShip.IsLandedOnObstacle = false;
            inReposition = false;
            MonoBehaviour.Destroy(ShipStand);
            Game.Movement.CollidedWith = null;
            MovementTemplates.HideLastMovementRuler();

            PreviousSubPhase.Resume();
        }

        private void StopDrag()
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;
        }

        private bool TryConfirmBarrelRollPosition(Ship.GenericShip ship)
        {
            bool allow = true;

            if (Game.Movement.CollidedWith != null)
            {
                Messages.ShowError("Cannot collide with another ships");
                allow = false;
            }
            else if (ship.IsLandedOnObstacle)
            {
                Messages.ShowError("Cannot land on Asteroid");
                allow = false;
            }
            else if (!BoardManager.ShipStandIsInside(ShipStand, BoardManager.BoardTransform.Find("Playmat")))
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

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            return false;
        }

    }

    public class BarrelRollExecutionSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;

        private bool performingAnimation;

        private GameObject ShipStand;
        private float helperDirection;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Barrel Roll execution";
            IsTemporary = true;
            UpdateHelpInfo();

            StartBarrelRollExecution();
        }

        private void StartBarrelRollExecution()
        {
            ShipStand = (PreviousSubPhase as BarrelRollPlanningSubPhase).ShipStand;
            helperDirection = (PreviousSubPhase as BarrelRollPlanningSubPhase).helperDirection;

            progressCurrent = 0;
            progressTarget = Vector3.Distance(Selection.ThisShip.GetPosition(), ShipStand.transform.position);

            Sounds.PlayFly();

            performingAnimation = true;
        }

        public override void Update()
        {
            if (performingAnimation) DoBarrelRollAnimation();
        }

        private void DoBarrelRollAnimation()
        {
            float progressStep = 0.5f * Time.deltaTime;
            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), ShipStand.transform.position, progressStep));
            progressCurrent += progressStep;
            Selection.ThisShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, helperDirection);
            Selection.ThisShip.MoveUpwards(progressCurrent / progressTarget);
            if (progressCurrent >= progressTarget)
            {
                FinishBarrelRollAnimation();
            }
        }

        private void FinishBarrelRollAnimation()
        {
            performingAnimation = false;

            MonoBehaviour.Destroy(ShipStand);
            Game.Movement.CollidedWith = null;

            MovementTemplates.HideLastMovementRuler();
            MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

            Selection.ThisShip.ToggleShipStandAndPeg(true);
            Selection.ThisShip.FinishPosition(delegate() { });

            Phases.FinishSubPhase(typeof(BarrelRollExecutionSubPhase));
            callBack();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
