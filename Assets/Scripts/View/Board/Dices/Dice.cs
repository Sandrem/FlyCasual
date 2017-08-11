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

    private static Vector3 rotationSuccess = new Vector3(330f, 120f, 40f);
    private static Vector3 rotationCrit = new Vector3(330f, 120f, 120);

    private static Vector3 positionGround = new Vector3(0, -3.763676f, 0);

    private static bool modelRollingIsFinished;
    private static float RollingIsFinishedTimePassed;
    private static readonly float RollingIsFinishedTimePassedNeeded = 1f;

    private GameObject Model;

    public Dice(DiceKind type, DiceSide side = DiceSide.Unknown)
    {

        Model = SpawnDice(type);

        Sides = new List<DiceSide>();

        Sides.Add(DiceSide.Blank);
        Sides.Add(DiceSide.Blank);
        Sides.Add(DiceSide.Focus);
        Sides.Add(DiceSide.Focus);
        Sides.Add(DiceSide.Success);
        Sides.Add(DiceSide.Success);
        Sides.Add(DiceSide.Success);

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
        Transform diceSpawningPoint = DicesManager.DiceSpawningPoint;
        GameObject model = MonoBehaviour.Instantiate(prefabDiceType, diceSpawningPoint.transform.position, prefabDiceType.transform.rotation, diceSpawningPoint.transform);
        return model;
    }

    private void RandomizeDice()
    {
        Model.transform.Find("Dice").transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public void NoRoll()
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
        Transform diceSpawningPoint = DicesManager.DiceSpawningPoint;
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
            default:
                Debug.Log("NOT IMPLEMENTED");
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
