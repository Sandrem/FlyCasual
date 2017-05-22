using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class EndSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "End SubPhase";
            Game.UI.AddTestLogEntry(Name);

            NextSubPhase();
        }

        public override void NextSubPhase()
        {
            Phases.CurrentPhase.NextPhase();
        }

    }

}
