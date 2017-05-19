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

        if (Game.Selection == null) Game.Selection = GameObject.Find("GameManager").GetComponent<ShipSelectionManagerScript>();

        if (Game.Selection.ThisShip != null)
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
        //if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        //{
        ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, Game.Selection.ThisShip.Model.GetPosition(), Game.Selection.ThisShip.Model.GetRotation(), Board.GetBoard());
        Roster.SetRaycastTargets(false);
        inReposition = true;
        //}
    }

    private void PerformDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
            {
                Game.Selection.ThisShip.Model.SetPosition(new Vector3(hit.point.x, 0.03f, hit.point.z));
            }
            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
            {
                ShipStand.transform.position = new Vector3(hit.point.x, 0.03f, hit.point.z);
            }
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            //Write Relative position
            Vector3 newPosition = Game.Selection.ThisShip.Model.InverseTransformPoint(ShipStand.transform.position);
            Vector3 fixedPositionRel = newPosition;

            //Transform currentHelper = Game.Selection.ThisShip.Model.GetHelper("Right");

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

                /*currentHelper = Game.Selection.ThisShip.Model.GetHelper("Right");
                currentHelper.gameObject.SetActive(true);
                Game.Selection.ThisShip.Model.GetHelper("Left").gameObject.SetActive(false);*/
            }

            if (newPosition.x < 0f) {
                fixedPositionRel = new Vector3(-2, fixedPositionRel.y, fixedPositionRel.z);

                /*currentHelper = Game.Selection.ThisShip.Model.GetHelper("Left");
                currentHelper.gameObject.SetActive(true);
                Game.Selection.ThisShip.Model.GetHelper("Right").gameObject.SetActive(false);*/
            }

            /*Vector3 helperPositionRel = Game.Selection.ThisShip.Model.InverseTransformPoint(currentHelper.position);

            if (fixedPositionRel.z-0.25 < helperPositionRel.z)
            {
                helperPositionRel = new Vector3(helperPositionRel.x, helperPositionRel.y, fixedPositionRel.z);
                Vector3 helperPositionAbs = ShipStand.transform.TransformPoint(helperPositionRel);
                currentHelper.position = helperPositionAbs;
            }
            if (fixedPositionRel.z-0.75 > helperPositionRel.z)
            {
                helperPositionRel = new Vector3(helperPositionRel.x, helperPositionRel.y, fixedPositionRel.z);
                Vector3 helperPositionAbs = ShipStand.transform.TransformPoint(helperPositionRel);
                currentHelper.position = helperPositionAbs;
            }*/

            Vector3 fixedPositionAbs = Game.Selection.ThisShip.Model.TransformPoint(fixedPositionRel);
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
            if (!ship.Model.IsInside(startingZone))
            {
                Game.UI.ShowError("Place ship into highlighted area");
                result = false;
            }
        }

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.BarrelRollSubPhase))
        {
            RollingShip = Game.Selection.ThisShip;
            Phases.StartMovementExecutionSubPhase("");
            inBarrelRoll = true;
            progressCurrent = 0;
            progressTarget = Vector3.Distance(RollingShip.Model.GetPosition(), ShipStand.transform.position);
            result = true;
        } 

        if (Game.Movement.CollidedWith != null)
        {
            Game.UI.ShowError("This ship shouldn't collide with another ships");
            result = false;
        }

        if (result) StopDrag();
        return result;
    }

    private void StopDrag()
    {
        Game.Selection.DeselectThisShip();
        Roster.SetRaycastTargets(true);
        inReposition = false;
        //Should I change subphase immediately?
        Phases.CurrentSubPhase.NextSubPhase();
    }

    //TODO: Move using curve
    private void BarrelRollAnimation()
    {
        float progressStep = 0.1f * Time.deltaTime;
        RollingShip.Model.SetPosition(Vector3.MoveTowards(RollingShip.Model.GetPosition(), ShipStand.transform.position, progressStep));
        progressCurrent += progressStep;
        RollingShip.Model.RotateModelDuringBarrelRoll(progressCurrent/progressTarget);
        RollingShip.Model.MoveUpwards(progressCurrent / progressTarget);
        if (progressCurrent >= progressTarget)
        {
            inBarrelRoll = false;
            Destroy(ShipStand);
            Phases.Next();
        }
    }

}
