using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

public class ObstaclesStayDetectorForced: MonoBehaviour {

    public bool checkCollisionsNow = false;

    public bool OverlapsShipNow
    {
        get { return OverlappedShipsNow.Count > 0; }
    }

    public List<GenericShip> OverlappedShipsNow = new List<GenericShip>();
    public bool OverlapsAsteroidNow = false;
    public bool OffTheBoardNow = false;
    public List<Collider> OverlapedMinesNow = new List<Collider>();

    void OnTriggerEnter(Collider collisionInfo)
    {

    }

    public void ReCheckCollisionsStart()
    {
        OverlapsAsteroidNow = false;
        OverlappedShipsNow = new List<GenericShip>();
        OffTheBoardNow = false;
        OverlapedMinesNow = new List<Collider>();

        checkCollisionsNow = true;
    }

    public void ReCheckCollisionsFinish()
    {
        checkCollisionsNow = false;
    }

    void OnTriggerStay(Collider collisionInfo)
    {
        if (checkCollisionsNow)
        {
            if (collisionInfo.tag == "Asteroid")
            {
                OverlapsAsteroidNow = true;
            }
            else if (collisionInfo.tag == "Mine")
            {
                if (!OverlapedMinesNow.Contains(collisionInfo)) OverlapedMinesNow.Add(collisionInfo);
            }
            else if (collisionInfo.name == "OffTheBoard")
            {
                OffTheBoardNow = true;
            }
            else if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != "Untagged" && collisionInfo.tag != Selection.ThisShip.GetTag())
                {
                    OverlappedShipsNow.Add(Roster.GetShipById(collisionInfo.tag));
                }
            }
        }
    }

    void OnTriggerExit(Collider collisionInfo)
    {

    }

}
