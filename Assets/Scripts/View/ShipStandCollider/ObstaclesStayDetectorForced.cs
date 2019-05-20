using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Obstacles;

public class ObstaclesStayDetectorForced: MonoBehaviour {

    public bool checkCollisionsNow = false;

    public bool OverlapsShipNow
    {
        get { return OverlappedShipsNow.Count > 0; }
    }

    public bool OverlapsAsteroidNow
    {
        get { return OverlappedAsteroidsNow.Count > 0; }
    }

    public List<GenericShip> OverlappedShipsNow = new List<GenericShip>();
    public bool OffTheBoardNow = false;
    public List<Collider> OverlapedMinesNow = new List<Collider>();
    public List<GenericObstacle> OverlappedAsteroidsNow = new List<GenericObstacle>();
    public bool OverlapsCurrentShipNow { get; private set; }

    private GenericShip theShip; 
    public GenericShip TheShip {
        get {
            return theShip ?? Selection.ThisShip;
        }
        set {
            theShip = value;
        }
    }

    public void ReCheckCollisionsStart()
    {
        OverlappedShipsNow = new List<GenericShip>();
        OffTheBoardNow = false;
        OverlapedMinesNow = new List<Collider>();
        OverlappedAsteroidsNow = new List<GenericObstacle> ();
        OverlapsCurrentShipNow = false;

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
            if (collisionInfo.tag == "Obstacle")
            {
                GenericObstacle obstacle = ObstaclesManager.GetChosenObstacle(collisionInfo.transform.name);
                if (!OverlappedAsteroidsNow.Contains(obstacle)) OverlappedAsteroidsNow.Add(obstacle);
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
                if (collisionInfo.tag != TheShip.GetTag())
                {
                    OverlappedShipsNow.Add(Roster.GetShipById(collisionInfo.tag));
                }
                else if (collisionInfo.tag == TheShip.GetTag())
                {
                    OverlapsCurrentShipNow = true;
                }
            }
        }
    }

}
