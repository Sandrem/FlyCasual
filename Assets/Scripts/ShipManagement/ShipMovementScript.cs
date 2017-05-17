using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ManeuverSpeed
{
    AdditionalMovement,
    Speed1,
    Speed2,
    Speed3,
    Speed4,
    Speed5
}

public enum ManeuverDirection
{
    Left,
    Forward,
    Right
}

public enum ManeuverBearing
{
    Straight,
    Bank,
    Turn,
    KoiogranTurn
}

[System.Serializable]
public class Movement
{
    public ManeuverSpeed Speed;
    public ManeuverDirection Direction;
    public ManeuverBearing Bearing;
    public Ship.ManeuverColor ColorComplexity;
}

[System.Serializable]
public struct MovementExecutionData
{
    public bool IsMoving;

    public int Speed;
    public ManeuverDirection MovementDirection;
    public ManeuverBearing MovementBearing;

    public float TargetProgress;
    public float CurrentProgress;


    public float TurningAroundDistance;

    public float AnimationSpeed;

    public bool CollisionReverting;
}

public class ShipMovementScript : MonoBehaviour {

    public RulerManagement Ruler;

    private float Delay;

    private GameManagerScript Game;

    private Movement CurrentMovement;
    public MovementExecutionData CurrentMovementData = new MovementExecutionData();
    private MovementExecutionData PreviousMovementData = new MovementExecutionData();

    private float moveDistance1 = 0f;
    private readonly float[] TURN_POINTS = new float[] { 2f, 3.6f, 5.2f };
    private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

    public Collider CollidedWith;

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    void Update () {
		UpdateMovement ();
    }

    //Assignment and launch of execution of meneuver

    public void AssignManeur()
    {
        string parameters = EventSystem.current.currentSelectedGameObject.name;
        Game.Selection.ThisShip.AssignedManeuver = ManeuverFromString(parameters);
        Game.Selection.ThisShip.InfoPanel.transform.FindChild("DialAssigned" + PlayerToInt(Game.Selection.ThisShip.PlayerNo)).gameObject.SetActive(true);

        Game.Selection.ThisShip.IsManeurPerformed = false;
        
        Game.UI.HideDirectionMenu();
    }

    public Movement ManeuverFromString(string parameters)
    {
        string[] arrParameters = parameters.Split('.');

        Movement result = new Movement();
        
        switch (arrParameters[0])
        {
            case "1":
                result.Speed = ManeuverSpeed.Speed1;
                break;
            case "2":
                result.Speed = ManeuverSpeed.Speed2;
                break;
            case "3":
                result.Speed = ManeuverSpeed.Speed3;
                break;
            case "4":
                result.Speed = ManeuverSpeed.Speed4;
                break;
            case "5":
                result.Speed = ManeuverSpeed.Speed5;
                break;
        }

        switch (arrParameters[1])
        {
            case "F":
                result.Direction = ManeuverDirection.Forward;
                break;
            case "L":
                result.Direction = ManeuverDirection.Left;
                break;
            case "R":
                result.Direction = ManeuverDirection.Right;
                break;
        }

        switch (arrParameters[2])
        {
            case "S":
                result.Bearing = ManeuverBearing.Straight;
                break;
            case "R":
                result.Bearing = ManeuverBearing.KoiogranTurn;
                break;
            case "B":
                result.Bearing = ManeuverBearing.Bank;
                break;
            case "T":
                result.Bearing = ManeuverBearing.Turn;
                break;
        }

        result.ColorComplexity = Game.Selection.ThisShip.GetColorComplexityOfManeuver(parameters);

        return result;
    }
    
    public void PerformStoredManeuver()
    {
        PerformMove(Game.Selection.ThisShip.AssignedManeuver);
	}

	private void PerformMove(Movement movementParameters) {

        Game.UI.HideContextMenu();

        Game.Selection.isUIlocked = true;

        CurrentMovementData.MovementBearing = movementParameters.Bearing;
        CurrentMovementData.MovementDirection = movementParameters.Direction;
        CurrentMovementData.Speed = GetSpeedFromMovement(movementParameters);

        Game.Selection.ThisShip.StartMoving();

        CurrentMovementData.IsMoving = true;
        
        float animationSpeedMultiplier = CurrentMovementData.Speed;

        CurrentMovementData.CurrentProgress = 0f;

        if (movementParameters.Direction == ManeuverDirection.Forward)
        {
            Vector3 TargetPosition = new Vector3(0, 0, GetMovement1() + CurrentMovementData.Speed * GetMovement1());
            CurrentMovementData.TargetProgress = TargetPosition.z;
            CurrentMovementData.AnimationSpeed = 0.5f;
        }

        if (movementParameters.Bearing == ManeuverBearing.Bank)
        {
            CurrentMovementData.TurningAroundDistance = GetMovement1() * BANK_SCALES[CurrentMovementData.Speed - 1];
            CurrentMovementData.TargetProgress = 45f;
            CurrentMovementData.AnimationSpeed = 40f / animationSpeedMultiplier;
        }

        if (movementParameters.Bearing == ManeuverBearing.Turn)
        {
            CurrentMovementData.TurningAroundDistance = GetMovement1() * TURN_POINTS[CurrentMovementData.Speed - 1];
            CurrentMovementData.TargetProgress = 90f;
            CurrentMovementData.AnimationSpeed = 40f / animationSpeedMultiplier;
        }

    }

    private int GetSpeedFromMovement(Movement movement)
    {
        int result = 0;
        switch (movement.Speed)
        {
            case ManeuverSpeed.Speed1:
                result = 1;
                break;
            case ManeuverSpeed.Speed2:
                result = 2;
                break;
            case ManeuverSpeed.Speed3:
                result = 3;
                break;
            case ManeuverSpeed.Speed4:
                result = 4;
                break;
            case ManeuverSpeed.Speed5:
                result = 5;
                break;
            case ManeuverSpeed.AdditionalMovement:
                result = 0;
                break;
            default:
                break;
        }
        return result;
    }

    // Movement itself

    private void UpdateMovement() {

        if (CurrentMovementData.IsMoving)
        {
            if (CurrentMovementData.MovementDirection == ManeuverDirection.Forward)
            {
                float progressDelta = CurrentMovementData.AnimationSpeed * Time.deltaTime;
                progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(CurrentMovementData.TargetProgress - CurrentMovementData.CurrentProgress));

                Vector3 progressDirection = (CurrentMovementData.CollisionReverting) ? Vector3.back : Vector3.forward;

                Game.Selection.ThisShip.Model.SetPosition(Vector3.MoveTowards(Game.Selection.ThisShip.Model.GetPosition(), Game.Selection.ThisShip.Model.GetPosition() + Game.Selection.ThisShip.Model.TransformDirection(progressDirection), progressDelta));
                CurrentMovementData.CurrentProgress += progressDelta;

                Game.Selection.ThisShip.Model.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);

                UpdateRotationFinisher();

                CheckCollisionsAfterStraight();
            }

            if (CurrentMovementData.MovementDirection != ManeuverDirection.Forward) {

                float progressDelta = CurrentMovementData.AnimationSpeed * Time.deltaTime;
                progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(CurrentMovementData.TargetProgress - CurrentMovementData.CurrentProgress));

                float turningDirection = (CurrentMovementData.MovementDirection == ManeuverDirection.Right) ? 1 : -1;
                int progressDirection = (CurrentMovementData.CollisionReverting) ? -1 : 1;

                Game.Selection.ThisShip.Model.Rotate(Game.Selection.ThisShip.Model.TransformPoint(new Vector3(CurrentMovementData.TurningAroundDistance * turningDirection, 0, 0)), turningDirection * progressDelta * progressDirection);
                CurrentMovementData.CurrentProgress += progressDelta;

                Game.Selection.ThisShip.Model.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);

                UpdateRotation();

                CheckCollisionsAfterNonStraight();
            }
            
        }
    }

    //Keeping ship's stand rotation according to game rules

    public void UpdateRotation()
    {

        float turningDirection = 0;
        if (CurrentMovementData.MovementDirection == ManeuverDirection.Right) turningDirection = 1;
        if (CurrentMovementData.MovementDirection == ManeuverDirection.Left) turningDirection = -1;

        Vector3 point_ShipStandBack = Game.Selection.ThisShip.Model.GetCentralBackPoint();
        Vector3 point_ShipStandFront = Game.Selection.ThisShip.Model.GetCentralFrontPoint();
        float pathToProcessLeft = (Ruler.movementRuler.transform.InverseTransformPoint(point_ShipStandBack).x);

        if (pathToProcessLeft > 0)
        {

            float distance_ShipStandFront_RulerStart = Vector3.Distance(Ruler.movementRuler.transform.position, point_ShipStandFront);
            float length_ShipStandFront_ShipStandBack = GetMovement1();
            Vector3 vector_RulerStart_ShipStandFront = Ruler.movementRuler.transform.InverseTransformPoint(point_ShipStandFront);
            Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

            float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

            float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
            float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

            float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
            float rotationFix = angle_ToShipFront_CorrectStandPosition * turningDirection;
            Game.Selection.ThisShip.Model.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));

            Vector3 standOrientationVector = Ruler.movementRuler.transform.InverseTransformPoint(Game.Selection.ThisShip.Model.GetCentralFrontPoint()) - Ruler.movementRuler.transform.InverseTransformPoint(Game.Selection.ThisShip.Model.GetCentralBackPoint());
            float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
            float angleFix = angleBetweenMinus * turningDirection;
            if (CurrentMovementData.CollisionReverting) angleFix = -angleFix;
            Game.Selection.ThisShip.Model.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));
        }

        Ruler.AddRulerCenterPoint(point_ShipStandFront);

    }

    public void UpdateRotationFinisher()
    {
        if (Ruler.movementRuler.transform.Find("Finisher") != null) {

            Vector3 point_ShipStandBack = Game.Selection.ThisShip.Model.GetCentralBackPoint();
            Vector3 point_ShipStandFront = Game.Selection.ThisShip.Model.GetCentralFrontPoint();

            float pathToProcessFinishingLeft = (Ruler.movementRuler.transform.Find("Finisher").InverseTransformPoint(point_ShipStandFront).x);
            
            if (pathToProcessFinishingLeft > 0)
            {
                float turningDirection = 0;
                if (PreviousMovementData.MovementDirection == ManeuverDirection.Right) turningDirection = 1;
                if (PreviousMovementData.MovementDirection == ManeuverDirection.Left) turningDirection = -1;

                Vector3 point_NearestMovementRulerCenter = Ruler.FindNearestRulerCenterPoint(point_ShipStandBack);

                Vector3 vector_ShipBackStand_ShipStandFront = point_ShipStandFront - point_ShipStandBack;
                Vector3 vector_NearestMovementRulerCenter_ShipStandFront = point_ShipStandFront - point_NearestMovementRulerCenter;
                float angle_ToShipStandBack_ToNearestMovementRulerCenter = Vector3.Angle(vector_ShipBackStand_ShipStandFront, vector_NearestMovementRulerCenter_ShipStandFront);

                Game.Selection.ThisShip.Model.SetRotationHelper2Angles(new Vector3(0, angle_ToShipStandBack_ToNearestMovementRulerCenter * turningDirection, 0));
            }

        }
    }

    //Non-straight movement finisher

    private void AdditionalMovement()
    {
        Game.Selection.ThisShip.Model.SimplifyRotationHelpers();

        PreviousMovementData = CurrentMovementData;

        Movement OneForward = new Movement()
        {
            Speed = ManeuverSpeed.AdditionalMovement,
            Direction = ManeuverDirection.Forward,
            Bearing = ManeuverBearing.Straight
        };

        PerformMove(OneForward);
    }

    //Collision checks

    private void CheckCollisionsAfterStraight()
    {
        if (CurrentMovementData.CollisionReverting)
        {
            if (CurrentMovementData.CurrentProgress != CurrentMovementData.TargetProgress)
            {
                if (CollidedWith == null)
                {
                    FinishCollidingWithSuccess();
                }
            }
            else
            {
                FinishCollidingWithFailure();
            }
        }
        else
        {
            if (CurrentMovementData.CurrentProgress == CurrentMovementData.TargetProgress)
            {
                if (CollidedWith == null)
                {
                    FinishWithoutColliding();
                }
                else
                {
                    RevertMove();
                }
            }
        }
    }

    private void CheckCollisionsAfterNonStraight()
    {
        //Check if additional movement is required
        if (CurrentMovementData.CurrentProgress == CurrentMovementData.TargetProgress)
        {
            AdditionalMovement();
        }

        if (CurrentMovementData.CollisionReverting)
        {
            if (CollidedWith == null)
            {
                FinishCollidingWithSuccess();
            }
        }
    }

    //Finish of movement

    private void FinishWithoutColliding()
    {
        Game.Selection.ThisShip.FinishMovingWithoutColliding();
        FinishMovement();
    }

    private void FinishCollidingWithSuccess()
    {
        Game.Selection.ThisShip.FinishMovingWithColliding();
        CurrentMovementData.CollisionReverting = false;
        Game.Selection.ThisShip.Model.ApplyRotationHelpers();
        FinishMovement();
    }

    private void FinishCollidingWithFailure() {
        //TODO: check work if ship cannot move at all
        CurrentMovementData = PreviousMovementData;
        RevertMove();
    }

	private void FinishMovement() {

        PreviousMovementData = new MovementExecutionData();

        Game.Selection.isUIlocked = false;

        Game.Selection.ThisShip.FinishMoving();

        CurrentMovementData.IsMoving = false;
        Game.Selection.ThisShip.Model.ResetRotationHelpers();
        
        
        Game.Selection.ThisShip.IsManeurPerformed = true;
        Game.Selection.ThisShip.IsAttackPerformed = false;

        Game.Selection.ThisShip.AssignedManeuver = null;

        if (!Game.PhaseManager.InTemporarySubPhase)
        {
            Game.PhaseManager.Next();
        }
    }

    private void RevertMove() {
        CurrentMovementData.CollisionReverting = true;
        CurrentMovementData.AnimationSpeed = CurrentMovementData.AnimationSpeed / 5;
        CurrentMovementData.CurrentProgress = 0f;
	}

    private float GetMovement1()
    {
        float result;
        if (moveDistance1 != 0)
        {
            result = moveDistance1;
        }
        else
        {
            result = Game.ShipFactory.Board.transform.TransformVector(new Vector3(4, 0, 0)).x;
        }
        return result;
    }

    //TODO: move
    public int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }

}
