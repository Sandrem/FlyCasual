using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

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
    private const float KOIOGRAN_ANIMATION_SPEED = 100;
    private int helperDirection;

    public GameObject prefabShipStand;

    private GameObject ShipStand;

    private Transform StartingZone;
    private bool isInsideStartingZone;

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
            PerformRotation();
            PerformDrag();
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollPlanningSubPhase))
        {
            Phases.CurrentSubPhase.Update();
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollExecutionSubPhase))
        {
            Phases.CurrentSubPhase.Update();
        }

        if (inKoiogranTurn)
        {
            DoKoiogranTurnAnimation();
        }

    }

    public void StartDrag()
    {
        if (Phases.CurrentPhase.GetType() == typeof(MainPhases.SetupPhase))
        {
            StartingZone = BoardManager.GetStartingZone(Phases.CurrentSubPhase.RequiredPlayer);
            isInsideStartingZone = false;
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            BoardManager.HighlightStartingZones();
            inReposition = true;
        }
    }

    private void PerformRotation()
    {
        CheckResetRotation();
        if (Input.GetKey(KeyCode.LeftControl))
        {
            RotateBy45();
        }
        else
        {
            RotateBy1();
        }

    }

    private void CheckResetRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 facing = (Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;
            Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -Selection.ThisShip.Model.transform.eulerAngles.y + facing.y, 0));
            Selection.ThisShip.ApplyRotationHelpers();
            Selection.ThisShip.ResetRotationHelpers();
        }
    }

    private void RotateBy45()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -45, 0));
            Selection.ThisShip.ApplyRotationHelpers();
            Selection.ThisShip.ResetRotationHelpers();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, 45, 0));
            Selection.ThisShip.ApplyRotationHelpers();
            Selection.ThisShip.ResetRotationHelpers();
        }
    }

    private static void RotateBy1()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -1, 0));
            Selection.ThisShip.ApplyRotationHelpers();
            Selection.ThisShip.ResetRotationHelpers();
        }

        if (Input.GetKey(KeyCode.E))
        {
            Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, 1, 0));
            Selection.ThisShip.ApplyRotationHelpers();
            Selection.ThisShip.ResetRotationHelpers();
        }
    }

    private void CheckControlledModeLimits()
    {
        // TODO: Rewrite

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            HideSetupHelpers();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            HideSetupHelpers();

            foreach (var ship in Selection.ThisShip.Owner.Ships)
            {
                if ((ship.Value.ShipId != Selection.ThisShip.ShipId) && (ship.Value.IsSetupPerformed))
                {
                    Vector3 newPosition = Selection.ThisShip.GetCenter();
                    float halfOfShipStandSize = BoardManager.BoardIntoWorld(BoardManager.DISTANCE_1 / 2f);
                    float oneOfShipStandSize = BoardManager.BoardIntoWorld(BoardManager.DISTANCE_1);

                    Dictionary<string, float> spaceBetweenList = GetSpaceBetween(Selection.ThisShip, ship.Value);

                    if ((spaceBetweenList["Left"] <= halfOfShipStandSize) && (spaceBetweenList["Left"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Up"] && spaceBetweenList["Up"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Down"] && spaceBetweenList["Down"] <= 0)))
                    {
                        Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Left" : "Right") ).gameObject.SetActive(true);
                        newPosition.x = newPosition.x - spaceBetweenList["Left"] + halfOfShipStandSize;
                    }
                    if ((spaceBetweenList["Right"] <= halfOfShipStandSize) && (spaceBetweenList["Right"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Up"] && spaceBetweenList["Up"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Down"] && spaceBetweenList["Down"] <= 0)))
                    {
                        Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Right" : "Left")).gameObject.SetActive(true);
                        newPosition.x = newPosition.x + spaceBetweenList["Right"] - halfOfShipStandSize;
                    }

                    if ((spaceBetweenList["Up"] <= halfOfShipStandSize) && (spaceBetweenList["Up"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Left"] && spaceBetweenList["Left"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Right"] && spaceBetweenList["Right"] <= 0)))
                    {
                        Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Top" : "Bottom")).gameObject.SetActive(true);
                        newPosition.z = newPosition.z + spaceBetweenList["Up"] - halfOfShipStandSize;
                    }
                    if ((spaceBetweenList["Down"] <= halfOfShipStandSize) && (spaceBetweenList["Down"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Left"] && spaceBetweenList["Left"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Right"] && spaceBetweenList["Right"] <= 0)))
                    {
                        Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Bottom" : "Top")).gameObject.SetActive(true);
                        newPosition.z = newPosition.z - spaceBetweenList["Down"] + halfOfShipStandSize;
                    }

                    Selection.ThisShip.SetCenter(newPosition);
                }
            }
        }
    }

    private void HideSetupHelpers()
    {
        foreach (Transform helper in Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers").transform)
        {
            helper.gameObject.SetActive(false);
        }
    }

    private Dictionary<string, float> GetSpaceBetween(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Dictionary<string, float> result = new Dictionary<string, float>();

        Dictionary<string, float> thisShipBounds = thisShip.GetBounds();
        Dictionary<string, float> anotherShipBounds = anotherShip.GetBounds();

        result.Add("Left", thisShipBounds["minX"] - anotherShipBounds["maxX"]);
        result.Add("Right", anotherShipBounds["minX"] - thisShipBounds["maxX"]);
        result.Add("Down", thisShipBounds["minZ"] - anotherShipBounds["maxZ"]);
        result.Add("Up", anotherShipBounds["minZ"] - thisShipBounds["maxZ"]);

        return result;
    }

    private void PerformDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
            {
                Selection.ThisShip.SetCenter(new Vector3(hit.point.x, 0f, hit.point.z));
            }
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            CheckControlledModeLimits();
            ApplySetupPositionLimits();
        }
    }

    private void ApplySetupPositionLimits()
    {
        Vector3 newPosition = Selection.ThisShip.GetCenter();
        Dictionary<string, float> newBounds = Selection.ThisShip.GetBounds();

        if (!isInsideStartingZone)
        {
            if ((newBounds["maxZ"] < StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z) && (newBounds["minZ"] > StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z))
            {
                isInsideStartingZone = true;
            }
        }
        
        if (isInsideStartingZone)
        {
            if (newBounds["maxZ"] > StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z) newPosition.z = StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z - (newBounds["maxZ"] - newPosition.z);
            if (newBounds["minZ"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z) newPosition.z = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z + (newPosition.z - newBounds["minZ"]);
        }
        
        if (newBounds["maxX"] > StartingZone.TransformPoint( 0.5f,  0.5f,  0.5f).x) newPosition.x = StartingZone.TransformPoint( 0.5f,  0.5f,  0.5f).x - (newBounds["maxX"] - newPosition.x);
        if (newBounds["minX"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x) newPosition.x = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x + (newPosition.x - newBounds["minX"]);

        Selection.ThisShip.SetCenter(newPosition);
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
            if (!ship.IsInside(StartingZone))

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

        //TODO: Different for setup and Barrel Roll
        if (result) StopDrag();

        return result;
    }

    private void StopDrag()
    {
        HideSetupHelpers();
        Roster.SetRaycastTargets(true);
        inReposition = false;

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
        {
            Selection.ThisShip.IsSetupPerformed = true;
            Selection.DeselectThisShip();
            BoardManager.TurnOffStartingZones();
        }

        Phases.Next();
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

        Selection.ThisShip.RotateAround(Selection.ThisShip.GetCenter(), progressStep);

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
