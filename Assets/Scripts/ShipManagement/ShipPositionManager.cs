using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositionManager : MonoBehaviour
{

    private GameManagerScript Game;
    public bool inReposition;

    //TEMP
    private bool inBarrelRoll;
    private Ship.GenericShip RollingShip;
    private float progressCurrent;
    private float progressTarget;
    private float helperDirection;

    public GameObject prefabShipStand;
    private GameObject ShipStand;

    // Use this for initialization
    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        if (Selection.ThisShip != null)
        {
            StartDrag();
        }

        if (inReposition)
        {
            PerformDrag();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //StopDrag();
        }

        if (inBarrelRoll)
        {
            BarrelRollAnimation();
        }

    }

    public void StartDrag()
    {
        if (Phases.CurrentPhase.GetType() == typeof(MainPhases.SetupPhase)) {
            Roster.SetRaycastTargets(false);
            inReposition = true;
        }
    }

    public void StartBarrelRoll()
    {
        ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation(), Board.GetBoard());

        ShipStand.transform.Find("ShipStandTemplate").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = Selection.ThisShip.Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material;
        
        Roster.SetRaycastTargets(false);
        inReposition = true;
        MovementTemplates.CurrentTemplate = MovementTemplates.GetMovement1Ruler();
        MovementTemplates.SaveCurrentMovementRulerPosition();
        MovementTemplates.CurrentTemplate.position = Selection.ThisShip.TransformPoint(new Vector3(0.5f, 0, -0.25f));
    }

    private void PerformDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
            {
                Selection.ThisShip.SetPosition(new Vector3(hit.point.x, 0.03f, hit.point.z));
            }
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
            {
                ShipStand.transform.position = new Vector3(hit.point.x, 0.03f, hit.point.z);
            }
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
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

            if (newPosition.x > 0f) {
                fixedPositionRel = new Vector3(2, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = 1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles + new Vector3(0, 180, 0);
            }

            if (newPosition.x < 0f) {
                fixedPositionRel = new Vector3(-2, fixedPositionRel.y, fixedPositionRel.z);

                helperDirection = -1f;
                MovementTemplates.CurrentTemplate.eulerAngles = Selection.ThisShip.Model.transform.eulerAngles;
            }

            Vector3 helperPositionRel = Selection.ThisShip.InverseTransformPoint(MovementTemplates.CurrentTemplate.position);
            helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, helperPositionRel.z);

            if (helperPositionRel.z+0.25f > fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.25f);
            }

            if (helperPositionRel.z+0.75f < fixedPositionRel.z)
            {
                helperPositionRel = new Vector3(helperDirection * Mathf.Abs(helperPositionRel.x), helperPositionRel.y, fixedPositionRel.z - 0.75f);
            }

            Vector3 helperPositionAbs = Selection.ThisShip.TransformPoint(helperPositionRel);
            MovementTemplates.CurrentTemplate.position = helperPositionAbs;

            Vector3 fixedPositionAbs = Selection.ThisShip.TransformPoint(fixedPositionRel);
            ShipStand.transform.position = fixedPositionAbs;
        }
    }

    //TODO: Good target to move into subphase class
    public bool TryConfirmPosition(Ship.GenericShip ship)
    {
        bool result = true;

        //TODO:
        //Cannot leave board
        //Obstacles

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            Transform startingZone = Board.GetStartingZone(Phases.CurrentSubPhase.RequiredPlayer);
            if (!ship.IsInside(startingZone))

            {
                Game.UI.ShowError("Place ship into highlighted area");
                result = false;
            }

            if (Game.Movement.CollidedWith != null)
            {
                Game.UI.ShowError("This ship shouldn't collide with another ships");
                result = false;
            }

        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            if (Game.Movement.CollidedWith == null)
            {
                StartBarrelRollAnimation(ship);
                result = true;
            }
            else
            {
                Game.UI.ShowError("This ship shouldn't collide with another ships");
                CancelBarrelRoll();
                result = false;
            }

        }

        if (result) StopDrag();
        return result;
    }

    private void StopDrag()
    {
        Selection.DeselectThisShip();
        Roster.SetRaycastTargets(true);
        inReposition = false;
        //Should I change subphase immediately?
        Phases.Next();
    }

    private void StartBarrelRollAnimation(Ship.GenericShip ship)
    {
        RollingShip = Selection.ThisShip;
        Phases.StartMovementExecutionSubPhase("");
        inBarrelRoll = true;
        progressCurrent = 0;
        progressTarget = Vector3.Distance(RollingShip.GetPosition(), ShipStand.transform.position);
        
    }

    //TODO: Move using curve
    private void BarrelRollAnimation()
    {
        float progressStep = 0.5f * Time.deltaTime;
        RollingShip.SetPosition(Vector3.MoveTowards(RollingShip.GetPosition(), ShipStand.transform.position, progressStep));
        progressCurrent += progressStep;
        RollingShip.RotateModelDuringBarrelRoll(progressCurrent/progressTarget, helperDirection);
        RollingShip.MoveUpwards(progressCurrent / progressTarget);
        if (progressCurrent >= progressTarget)
        {
            FinishBarrelRollAnimation();
        }
    }

    private void FinishBarrelRollAnimation()
    {
        inBarrelRoll = false;
        Destroy(ShipStand);
        Game.Movement.CollidedWith = null;
        MovementTemplates.HideLastMovementRuler();

        RollingShip.FinishPosition();

        Phases.Next();
    }

    private void CancelBarrelRoll()
    {
        inReposition = false;
        Destroy(ShipStand);
        Game.Movement.CollidedWith = null;
        MovementTemplates.HideLastMovementRuler();
        Actions.ShowActionsPanel();
    }

}
