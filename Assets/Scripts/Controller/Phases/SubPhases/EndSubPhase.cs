using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class EndSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "End SubPhase";

            Next();
        }

        public override void Next()
        {
            Phases.CurrentPhase.NextPhase();
        }

    }

}
