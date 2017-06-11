using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositionManager : MonoBehaviour
{

    private GameManagerScript Game;
    public bool inReposition;

    //TEMP
    private bool inBarrelRoll;
    private bool inKoiogranTurn;
    private Ship.GenericShip RollingShip;
    private float progressCurrent;
    private float progressTarget;
    private float helperDirection;
    private const float KOIOGRAN_ANIMATION_SPEED = 100;

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

        if (!inReposition) {
            if (Selection.ThisShip != null)
            {
                StartDrag();
            }
        }
        else
        {
            PerformDrag();
        }

        if (inBarrelRoll)
        {
            DoBarrelRollAnimation();
        }

        if (inKoiogranTurn)
        {
            DoKoiogranTurnAnimation();
        }

    }

    public void StartDrag()
    {
        if (Phases.CurrentPhase.GetType() == typeof(MainPhases.SetupPhase)) {
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            Board.HighlightStartingZones();
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
                Selection.ThisShip.SetPosition(new Vector3(hit.point.x, 0f, hit.point.z));
            }
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
            {
                ShipStand.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
            }
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            ApplySetupPositionLimits();
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            ApplyBarrelRollRepositionLimits();
        }
    }

    private void ApplySetupPositionLimits()
    {
        Vector3 newPosition = Selection.ThisShip.GetPosition();

        if (newPosition.z > 5) newPosition.z = 5;
        if (newPosition.z < -5) newPosition.z = -5;
        if (newPosition.x > 5) newPosition.x = 5;
        if (newPosition.x < -5) newPosition.x = -5;

        Selection.ThisShip.SetPosition(newPosition);
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
            TryConfirmBarrelRollPosition(ship);
            result = true;
        }

        //TODO: Different for setup and Barrel Roll
        if (result) StopDrag();

        return result;
    }

    private void TryConfirmBarrelRollPosition(Ship.GenericShip ship)
    {
        bool allow = true;

        if (Game.Movement.CollidedWith != null)
        {
            Game.UI.ShowError("Cannot collide with another ships");
            allow = false;
        }
        else if (ship.ObstaclesLanded.Count != 0)
        {
            Game.UI.ShowError("Cannot land on Asteroid");
            allow = false;
        }
        else if (!ShipStandIsInside(Board.BoardTransform.Find("Playmat")))
        {
            Game.UI.ShowError("Cannot leave the battlefield");
            allow = false;
        }

        if (allow)
        {
            StartBarrelRollAnimation(ship);
        }
        else
        {
            CancelBarrelRoll();
        }
    }

    public bool ShipStandIsInside(Transform zone)
    {
        Vector3 zoneStart = zone.transform.TransformPoint(-0.5f, -0.5f, -0.5f);
        Vector3 zoneEnd = zone.transform.TransformPoint(0.5f, 0.5f, 0.5f);
        bool result = true;

        List<Vector3> shipStandEdges = new List<Vector3>
        {
            ShipStand.transform.TransformPoint(new Vector3(-0.5f, 0f, 0)),
            ShipStand.transform.TransformPoint(new Vector3(0.5f, 0f, 0)),
            ShipStand.transform.TransformPoint(new Vector3(-0.5f, 0f, -1f)),
            ShipStand.transform.TransformPoint(new Vector3(0.5f, 0f, -1))
        };

        foreach (var point in shipStandEdges)
        {
            if ((point.x < zoneStart.x) || (point.z < zoneStart.z) || (point.x > zoneEnd.x) || (point.z > zoneEnd.z))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    private void StopDrag()
    {
        Roster.SetRaycastTargets(true);
        inReposition = false;

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            Selection.ThisShip.IsSetupPerformed = true;
            Selection.DeselectThisShip();
            Board.TurnOffStartingZones();
        }

        Phases.Next();
    }

    private void StartBarrelRollAnimation(Ship.GenericShip ship)
    {
        RollingShip = Selection.ThisShip;
        Phases.StartRepositionExecutionSubPhase("");
        inBarrelRoll = true;
        progressCurrent = 0;
        progressTarget = Vector3.Distance(RollingShip.GetPosition(), ShipStand.transform.position);
        RollingShip.ToggleShipStandAndPeg(false);
        MovementTemplates.CurrentTemplate.gameObject.SetActive(false);

        Sounds.PlayFly();
    }

    //TODO: Move using curve
    private void DoBarrelRollAnimation()
    {
        float progressStep = 0.5f * Time.deltaTime;
        RollingShip.SetPosition(Vector3.MoveTowards(RollingShip.GetPosition(), ShipStand.transform.position, progressStep));
        progressCurrent += progressStep;
        RollingShip.RotateModelDuringBarrelRoll(progressCurrent / progressTarget, helperDirection);
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
        MovementTemplates.CurrentTemplate.gameObject.SetActive(true);

        RollingShip.ToggleShipStandAndPeg(true);
        RollingShip.FinishPosition();

        Phases.Next();
    }

    private void CancelBarrelRoll()
    {
        Selection.ThisShip.ObstaclesLanded = new List<Collider>();
        inReposition = false;
        Destroy(ShipStand);
        Game.Movement.CollidedWith = null;
        MovementTemplates.HideLastMovementRuler();

        Phases.Next();

        Actions.ShowActionsPanel();
    }

    public void StartKoiogranTurn()
    {
        progressCurrent = 0;
        progressTarget = 180;
        inKoiogranTurn = true;
    }

    private void DoKoiogranTurnAnimation()
    {
        float progressStep = Mathf.Min(Time.deltaTime*KOIOGRAN_ANIMATION_SPEED, progressTarget-progressCurrent);
        progressCurrent += progressStep;

        Selection.ThisShip.Rotate(Selection.ThisShip.GetCenter(), progressStep);

        float positionY = (progressCurrent < 90) ? progressCurrent : 180 - progressCurrent;
        positionY = positionY / 90;
        Selection.ThisShip.SetHeight(positionY);

        if (progressCurrent == progressTarget) EndKoiogranTurn();
    }

    private void EndKoiogranTurn()
    {
        inKoiogranTurn = false;
        Phases.FinishSubPhase(typeof(SubPhases.KoiogranTurnSubPhase));
    }

}
