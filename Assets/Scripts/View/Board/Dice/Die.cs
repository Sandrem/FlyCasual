using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceKind
{
    Attack,
    Defence
}

public partial class Die
{
    private DiceRoll ParentDiceRoll;
    private DiceKind Type;

    private static Vector3 rotationCrit = new Vector3(325f, 120f, 135f);
    private static Vector3 rotationSuccess = new Vector3(330f, 120f, 40f);
    private static Vector3 rotationFocus = new Vector3(40f, -63f, 40f);
    private static Vector3 rotationBlank = new Vector3(-40f, 0f, -45f);

    private static Vector3 positionGround = new Vector3(0, -3.763676f, 0);

    private static bool modelRollingIsFinished;
    private static float RollingIsFinishedTimePassed;
    private static readonly float RollingIsFinishedTimePassedNeeded = 1f;

    private static int diceIDcounter;

    public GameObject Model { get; private set; }

    public Die(DiceRoll diceRoll, DiceKind type, DieSide side = DieSide.Unknown)
    {
        ParentDiceRoll = diceRoll;
        Type = type;
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

        if (type == DiceKind.Attack) Sides.Add(DieSide.Crit);
        if (type == DiceKind.Defence) Sides.Add(DieSide.Blank);

        if (side != DieSide.Unknown)
        {
            Side = side;
        }
        else
        {
            Side = Sides[Random.Range(0, 8)];
        }
    }

    private GameObject SpawnDice(DiceKind type)
    {
        GameObject prefabDiceType = (type == DiceKind.Attack) ? DiceManager.DiceAttack : DiceManager.DiceDefence;
        Transform diceSpawningPoint = ParentDiceRoll.SpawningPoint;
        GameObject model = MonoBehaviour.Instantiate(prefabDiceType, diceSpawningPoint.transform.position, prefabDiceType.transform.rotation, diceSpawningPoint.transform);
        model.name = "DiceN" + diceIDcounter++;
        return model;
    }

    public void RandomizeRotation()
    {
        SetInitialRotation(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
    }

    public void ShowWithoutRoll()
    {
        if (Model == null) Model = SpawnDice(Type);
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").transform.localPosition = positionGround;
    }

    public void SetInitialRotation(Vector3 rotationAngles)
    {
        if (Model == null) Model = SpawnDice(Type);
        Model.transform.Find("Dice").transform.eulerAngles = new Vector3(rotationAngles.x, rotationAngles.y, rotationAngles.z);
    }

    public void Roll()
    {
        if (Model == null) Model = SpawnDice(Type);
        modelRollingIsFinished = false;
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").GetComponent<Rigidbody>().isKinematic = false;
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
        switch (newSide)
        {
            case DieSide.Success:
                Model.transform.Find("Dice").localEulerAngles = rotationSuccess;
                break;
            case DieSide.Crit:
                Model.transform.Find("Dice").localEulerAngles = rotationCrit;
                break;
            case DieSide.Focus:
                Model.transform.Find("Dice").localEulerAngles = rotationFocus;
                break;
            case DieSide.Blank:
                Model.transform.Find("Dice").localEulerAngles = rotationBlank;
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
        if (!modelRollingIsFinished)
        {
            Vector3 velocity = Model.GetComponentInChildren<Rigidbody>().velocity;
            bool isModelRollingNow = (velocity != Vector3.zero);

            RollingIsFinishedTimePassed = (isModelRollingNow) ? 0 : RollingIsFinishedTimePassed + Time.deltaTime;

            if (RollingIsFinishedTimePassed >= RollingIsFinishedTimePassedNeeded)
            {
                modelRollingIsFinished = true;
                RollingIsFinishedTimePassed = 0;
            }
        }

        return modelRollingIsFinished;
    }

}
