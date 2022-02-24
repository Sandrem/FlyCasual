using System.Collections;
using System.Collections.Generic;
using Movement;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;
using UnityEngine;

namespace Ship.FirstEdition.AWing
{
    public class AWing : GenericShip
    {
        public AWing() : base()
        {
            

            HotacManeuverTable = new AI.AWingTable();
        }
    }
}
