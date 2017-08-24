using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceKind
{
    Attack,
    Defence
}

public partial class Dice
{
    private DiceRoll ParentDiceRoll;

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

    public Dice(DiceRoll diceRoll, DiceKind type, DiceSide side = DiceSide.Unknown)
    {
        ParentDiceRoll = diceRoll;

        Model = SpawnDice(type);

        Sides = new List<DiceSide>
        {
            DiceSide.Blank,
            DiceSide.Blank,
            DiceSide.Focus,
            DiceSide.Focus,
            DiceSide.Success,
            DiceSide.Success,
            DiceSide.Success
        };

        if (type == DiceKind.Attack) Sides.Add(DiceSide.Crit);
        if (type == DiceKind.Defence) Sides.Add(DiceSide.Blank);

        if (side != DiceSide.Unknown)
        {
            Side = side;
            //TODO: Not only success
            Model.transform.Find("Dice").localEulerAngles = rotationSuccess;
        }
        else
        {
            Side = Sides[Random.Range(0, 8)];
        }
    }

    private GameObject SpawnDice(DiceKind type)
    {
        GameObject prefabDiceType = (type == DiceKind.Attack) ? DicesManager.DiceAttack : DicesManager.DiceDefence;
        Transform diceSpawningPoint = ParentDiceRoll.SpawningPoint;
        GameObject model = MonoBehaviour.Instantiate(prefabDiceType, diceSpawningPoint.transform.position, prefabDiceType.transform.rotation, diceSpawningPoint.transform);
        model.name = "DiceN" + diceIDcounter++;
        return model;
    }

    private void RandomizeDice()
    {
        Model.transform.Find("Dice").transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public void ShowWithoutRoll()
    {
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").transform.localPosition = positionGround;
    }

    public void Roll()
    {
        modelRollingIsFinished = false;
        RandomizeDice();
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

    public void SetModelSide(DiceSide newSide)
    {
        switch (newSide)
        {
            case DiceSide.Success:
                Model.transform.Find("Dice").localEulerAngles = rotationSuccess;
                break;
            case DiceSide.Crit:
                Model.transform.Find("Dice").localEulerAngles = rotationCrit;
                break;
            case DiceSide.Focus:
                Model.transform.Find("Dice").localEulerAngles = rotationFocus;
                break;
            case DiceSide.Blank:
                Model.transform.Find("Dice").localEulerAngles = rotationBlank;
                break;
        }
    }

    public void SetPosition(Vector3 position)
    {
        Model.transform.Find("Dice").GetComponent<Rigidbody>().isKinematic = true;
        Model.transform.Find("Dice").position = new Vector3 (position.x, Model.transform.Find("Dice").position.y, position.z);
    }

    public DiceSide GetModelFace()
    {
        string resultName = "";
        DiceSide resultSide = DiceSide.Unknown;
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
                resultSide = DiceSide.Blank;
                break;
            case "focus":
                resultSide = DiceSide.Focus;
                break;
            case "success":
                resultSide = DiceSide.Success;
                break;
            case "crit":
                resultSide = DiceSide.Crit;
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
