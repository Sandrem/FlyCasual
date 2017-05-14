using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositionManager : MonoBehaviour
{

    private GameManagerScript Game;
    public bool inReposition;

    public GameObject StartingZone1;
    public GameObject StartingZone2;

    // Use this for initialization
    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
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

    }

    private void StartDrag()
    {
        if (Game.PhaseManager.CurrentPhase.GetType() == typeof(Phases.SetupPhase)) {
            Game.Roster.SetRaycastTargets(false);
            inReposition = true;
        }
    }

    private void PerformDrag()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Game.Selection.ThisShip.Model.SetPosition(new Vector3(hit.point.x, 0.03f, hit.point.z));
        }
    }

    public bool TryConfirmPosition(Ship.GenericShip ship)
    {
        bool result = true;

        GameObject startingZone = (Game.PhaseManager.CurrentSubPhase.RequiredPlayer == Player.Player1) ? StartingZone1 : StartingZone2;
        if (!ship.Model.IsInside(startingZone.transform))
        {
            Game.UI.ShowError("Place ship into highlighted area");
            result = false;
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
        Game.Roster.SetRaycastTargets(true);
        inReposition = false;
        //Should I change subphase immediately?
        Game.PhaseManager.CurrentSubPhase.NextSubPhase();
    }

    public void HighlightStartingZones()
    {
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        StartingZone1.SetActive(false);
        StartingZone2.SetActive(false);
        
        //fix
        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        if (Game.PhaseManager.CurrentPhase.GetType() == typeof(Phases.SetupPhase))
        {
            if (Game.PhaseManager.CurrentSubPhase.RequiredPlayer == Player.Player1) StartingZone1.SetActive(true);
            if (Game.PhaseManager.CurrentSubPhase.RequiredPlayer == Player.Player2) StartingZone2.SetActive(true);
        }
    }

}
