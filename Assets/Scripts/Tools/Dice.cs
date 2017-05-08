using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{

    private DiceManagementScript DiceManager;

    private static Vector3 rotationSuccess = new Vector3(330f, 120f, 40f);
    private static Vector3 rotationCrit = new Vector3(330f, 120f, 120);

    private static Vector3 positionGround = new Vector3(0, -3.763676f, 0);

    public string Type
    {
        get;
        private set;
    }

    public DiceSide Side
    {
        get;
        private set;
    }

    private List<DiceSide> Sides
    {
        get;
        set;
    }

    private GameObject Model;

    public Dice(string type, DiceSide side = DiceSide.Unknown)
    {
        DiceManager = GameObject.Find("GameManager").GetComponent<DiceManagementScript>();

        Type = type;
        Model = SpawnDice(type);

        Sides = new List<DiceSide>();

        Sides.Add(DiceSide.Blank);
        Sides.Add(DiceSide.Blank);
        Sides.Add(DiceSide.Focus);
        Sides.Add(DiceSide.Focus);
        Sides.Add(DiceSide.Success);
        Sides.Add(DiceSide.Success);
        Sides.Add(DiceSide.Success);

        if (type == "attack") Sides.Add(DiceSide.Crit);
        if (type == "defence") Sides.Add(DiceSide.Blank);

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

    private GameObject SpawnDice(string type)
    {
        GameObject prefabDiceType = (type == "attack") ? DiceManager.diceAttack : DiceManager.diceDefence;
        GameObject diceSpawningPoint = DiceManager.diceSpawningPoint;
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
        RandomizeDice();
        Model.gameObject.SetActive(true);
        Model.transform.Find("Dice").GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Reroll()
    {
        GameObject diceSpawningPoint = DiceManager.diceSpawningPoint;
        Model.transform.Find("Dice").transform.position = diceSpawningPoint.transform.position;
        Roll();
    }

    public void Cancel()
    {
        Side = DiceSide.Blank;
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

    public void SetSide(DiceSide side)
    {
        Side = side;
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

}
