using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipFactoryScript : MonoBehaviour {

    private GameManagerScript Game;

    public GameObject prefabShip;
    public GameObject Board;

	private static Vector3 ROTATION_FORWARD = new Vector3 (0, 0, 0);
	private static Vector3 ROTATION_BACKWARD = new Vector3 (0, 180, 0);

	public Material StandRed;
	public Material StandGreen;

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

	// Update is called once per frame
	void Update () {

	}

	//TODO: REWRITE ASAP
	public Ship.GenericShip SpawnShip(string pilotName, Player player, int id, Vector3 position) {

        if (Game == null) Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Ship.GenericShip newShipContainer = (Ship.GenericShip) System.Activator.CreateInstance(System.Type.GetType(pilotName), player, id, position);

        newShipContainer.InfoPanel = this.gameObject.GetComponent<RosterInfoScript>().CreateRosterInfo(newShipContainer);

        newShipContainer.OnDestroyed += Game.Rules.WinConditions.CheckWinConditions;
        newShipContainer.AfterGotNumberOfAttackDices += Game.Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDices += Game.Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.OnTryPerformAction += Game.Rules.Stress.CanPerformActions;
        newShipContainer.OnTryPerformAction += Game.Rules.DuplicatedActions.CanPerformActions;
        newShipContainer.OnMovementFinishWithoutColliding += Game.Rules.KoiogranTurn.CheckKoiogranTurn;
        newShipContainer.OnMovementFinishWithColliding += Game.Rules.KoiogranTurn.CheckKoiogranTurnError;
        newShipContainer.OnMovementStart += Game.Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithoutColliding += Game.Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithColliding += Game.Rules.Collision.AssignCollision;
        newShipContainer.OnMovementStart += Game.Ruler.ApplyMovementRuler;
        newShipContainer.OnMovementStart += Game.Ruler.CallReturnRangeRuler;
        newShipContainer.OnMovementFinish += Game.Ruler.ResetRuler;
        newShipContainer.OnMovementFinish += Game.Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementFinish += Game.Rules.Stress.CheckStress;
        newShipContainer.AfterGetManeuverAvailablity += Game.Rules.Stress.CannotPerformRedManeuversWhileStressed;

        Game.PhaseManager.OnPlanningPhaseStart += newShipContainer.ClearAlreadyExecutedActions;

        newShipContainer.AfterTokenIsAssigned += Game.UI.Roster.UpdateTokensIndicator;
        newShipContainer.AfterTokenIsRemoved += Game.UI.Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Game.UI.Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Game.UI.Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Game.UI.Roster.UpdateShipStats;

        return newShipContainer;
	}

    public GameObject CreateShipModel(Ship.GenericShip newShipContainer, Vector3 position) {
        Vector3 facing = (newShipContainer.PlayerNo == Player.Player1) ? ROTATION_FORWARD : ROTATION_BACKWARD;
        GameObject newShip = Instantiate(prefabShip, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.transform);
        newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(newShipContainer.Type).gameObject.SetActive(true);

        if (newShipContainer.PlayerNo == Player.Player2)
        {
            newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").GetComponent<Renderer>().material = StandGreen;
            newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipPeg").GetComponent<Renderer>().material = StandGreen;
        }

        SetTagOfChildren(newShip.transform, "ShipId:" + newShipContainer.ShipId.ToString());

        //Check Size of stand
        //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
        //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));

        //Check size of playmat
        //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
        //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));


        return newShip;
    }

    private void SetTagOfChildren(Transform parent, string tag)
    {
        parent.gameObject.tag = tag;
        if (parent.childCount > 0)
        {
            foreach (Transform t in parent)
            {
                SetTagOfChildren(t, tag);
            }
        }
    }

}
