using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesStayDetectorForced: MonoBehaviour {

    public bool checkCollisionsNow = false;

    public bool OverlapsShipNow = false;
    public bool OverlapsAsteroidNow = false;
    public bool OffTheBoardNow = false;
    public List<Collider> OverlapedMines = new List<Collider>();

    void OnTriggerEnter(Collider collisionInfo)
    {

    }

    public void ReCheckCollisionsStart()
    {
        OverlapsAsteroidNow = false;
        OverlapsShipNow = false;
        OffTheBoardNow = false;
        OverlapedMines = new List<Collider>();

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
                if (!OverlapedMines.Contains(collisionInfo)) OverlapedMines.Add(collisionInfo);
            }
            else if (collisionInfo.name == "OffTheBoard")
            {
                OffTheBoardNow = true;
            }
            else if (collisionInfo.name == "ObstaclesStayDetector")
            {
                if (collisionInfo.tag != "Untagged" && collisionInfo.tag != Selection.ThisShip.GetTag())
                {
                    OverlapsShipNow = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider collisionInfo)
    {

    }

}
