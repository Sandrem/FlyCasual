using Players;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceKind
{
    Attack,
    Defence
}

public class DieRollEventArg : EventArgs
{
    public PlayerNo PlayerNo { get; private set; }
    public DiceKind DiceKind { get; private set; }

    public DieRollEventArg(PlayerNo playerNo, DiceKind diceKind)
    {
        PlayerNo = playerNo;
        DiceKind = diceKind;
    }
}

public class DieResultEventArg: EventArgs
{
    public PlayerNo PlayerNo { get; private set; }
    public DiceKind DiceKind { get; private set; }
    public DieSide DieSide { get; private set; }

    public DieResultEventArg(PlayerNo playerNo, DiceKind diceKind, DieSide dieSide)
    {
        PlayerNo = playerNo;
        DiceKind = diceKind;
        DieSide = dieSide;
    }
}

public partial class Die
{
    private DiceRoll ParentDiceRoll { get; set; }
    private DiceKind Type { get; set; }

    private static Vector3 ROTATION_CRIT { get; set; } = new Vector3(325f, 120f, 135f);
    private static Vector3 ROTATION_SUCCESS { get; set; } = new Vector3(330f, 120f, 40f);
    private static Vector3 ROTATION_FOCUS { get; set; } = new Vector3(40f, -63f, 40f);
    private static Vector3 ROTATION_BLANK { get; set; } = new Vector3(-40f, 0f, -45f);

    private static Vector3 POSITION_GROUND { get; set; } = new Vector3(0, -14.763676f, 0);

    private static bool ModelRollingIsFinished { get; set; }
    private static float RollingIsFinishedTimePassed { get; set; }
    private static readonly float TIME_TO_FINISH_ROLLING = 1f;

    private static int DiceIDCounter { get; set; }

    public GameObject Model { get; private set; }
    public bool IsWaitingForNewResult { get; set; }

    public Die(DiceRoll diceRoll, DiceKind diceKind, DieSide side = DieSide.Unknown)
    {
        ParentDiceRoll = diceRoll;
        Type = diceKind;
        IsUncancelable = false;
        Sides = new List<DieSide>
        {
            DieSide.Blank,
            DieSide.Blank,
            DieSide.Focus,
            DieSide.Focus,
            DieSide.Success,
            DieSide.Success,
            DieSide.Success
        };

        if (Type == DiceKind.Attack) Sides.Add(DieSide.Crit);
        if (Type == DiceKind.Defence) Sides.Add(DieSide.Blank);

        if (side != DieSide.Unknown)
        {
            Side = side;
            IsAddedResult = true;
        }
        else
        {
            Side = Sides[UnityEngine.Random.Range(0, 8)];
            IsAddedResult = false;
        }
    }

    private GameObject SpawnDice(DiceKind type)
    {
        GameObject prefabDiceType = (type == DiceKind.Attack) ? DiceManager.DiceAttack : DiceManager.DiceDefence;
        Transform diceSpawningPoint = ParentDiceRoll.SpawningPoint;
        GameObject model = MonoBehaviour.Instantiate(prefabDiceType, diceSpawningPoint.transform.position, prefabDiceType.transform.rotation, diceSpawningPoint.transform);
        model.name = "DiceN" + DiceIDCounter++;
        return model;
    }

    public void RandomizeRotation()
    {
        if (Model == null) Model = SpawnDice(Type);
        SetInitialRotation(new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        SetInitialRotationForce(new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100)));
    }

    public void ShowWithoutRoll()
    {
        if (Model == null) Model = SpawnDice(Type);
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").transform.localPosition = POSITION_GROUND;
    }

    public void SetInitialRotation(Vector3 rotationAngles)
    {
        Model.transform.Find("Dice").transform.eulerAngles = new Vector3(rotationAngles.x, rotationAngles.y, rotationAngles.z);
    }

    public void SetInitialRotationForce(Vector3 rotationForce)
    {
        Model.transform.Find("Dice").GetComponentInChildren<Rigidbody>().angularVelocity = new Vector3(rotationForce.x, rotationForce.y, rotationForce.z);
    }

    public void Roll()
    {
        if (Model == null) Model = SpawnDice(Type);
        Model.GetComponentInChildren<Rigidbody>().isKinematic = false;
        IsWaitingForNewResult = true;
        ModelRollingIsFinished = false;
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").GetComponent<Rigidbody>().isKinematic = false;

        DiceManager.CallDiceRolled(this, new DieRollEventArg(ParentDiceRoll.Owner, Type));
    }

    public void Reroll()
    {
        ToggleSelected(false);
        IsRerolled = true;

        Transform diceSpawningPoint = ParentDiceRoll.SpawningPoint;
        Model.transform.Find("Dice").transform.position = diceSpawningPoint.position;
        Roll();
    }

    public void SetModelSide(DieSide newSide)
    {
        if (Model == null) return;

        switch (newSide)
        {
            case DieSide.Success:
                Model.transform.Find("Dice").localEulerAngles = ROTATION_SUCCESS;
                break;
            case DieSide.Crit:
                Model.transform.Find("Dice").localEulerAngles = ROTATION_CRIT;
                break;
            case DieSide.Focus:
                Model.transform.Find("Dice").localEulerAngles = ROTATION_FOCUS;
                break;
            case DieSide.Blank:
                Model.transform.Find("Dice").localEulerAngles = ROTATION_BLANK;
                break;
        }
    }

    public void SetPosition(Vector3 position)
    {
        Model.transform.Find("Dice").GetComponent<Rigidbody>().isKinematic = true;
        Model.transform.Find("Dice").position = new Vector3 (position.x, Model.transform.Find("Dice").position.y, position.z);
    }

    public DieSide GetModelFace()
    {
        string resultName = "";
        DieSide resultSide = DieSide.Unknown;
        float resultHighest = float.MinValue;

        //TODO: Bug??? (TargetLock using)
        foreach (Transform face in Model.transform.Find("Dice"))
        {
            float currentHeight = face.TransformPoint(face.position).y;
            if (currentHeight > resultHighest)
            {
                resultName = face.name;
                resultHighest = currentHeight;
            }
        }

        switch (resultName)
        {
            case "empty":
                resultSide = DieSide.Blank;
                break;
            case "focus":
                resultSide = DieSide.Focus;
                break;
            case "success":
                resultSide = DieSide.Success;
                break;
            case "crit":
                resultSide = DieSide.Crit;
                break;
        }

        return resultSide;
    }

    public bool IsDiceFaceVisibilityWrong()
    {
        bool result = false;

        Vector3 vectorUp = Vector3.up;
        float lowest = float.MaxValue;
        Transform diceTransform = Model.transform.Find("Dice");

        if (diceTransform.localPosition.y >= -3.5)
        {
            result = true;
        }
        else
        {
            foreach (Transform face in diceTransform)
            {
                Vector3 faceVector = face.position - diceTransform.position;
                float angle = Vector3.Angle(vectorUp, faceVector);
                if (angle < lowest) lowest = angle;
            }
            if (lowest > 20)
            {
                result = true;
            }
        }

        return result;
    }

    public void RemoveModel()
    {
        MonoBehaviour.Destroy(Model);
    }

    public bool IsModelRollingFinished()
    {
        if (!ModelRollingIsFinished)
        {
            Vector3 velocity = Model.GetComponentInChildren<Rigidbody>().velocity;
            bool isModelRollingNow = (velocity != Vector3.zero);

            RollingIsFinishedTimePassed = (isModelRollingNow) ? 0 : RollingIsFinishedTimePassed + Time.deltaTime;

            if (RollingIsFinishedTimePassed >= TIME_TO_FINISH_ROLLING)
            {
                ModelRollingIsFinished = true;
                RollingIsFinishedTimePassed = 0;
            }
        }

        return ModelRollingIsFinished;
    }

}
