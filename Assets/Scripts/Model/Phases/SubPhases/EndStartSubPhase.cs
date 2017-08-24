using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class EndStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "End start";
        }

        public override void Initialize()
        {
            Phases.CallEndPhaseTrigger(EndRound);
        }

        private void EndRound()
        {
            Phases.CallRoundEndTrigger();
            Next();
        }

        public override void Next()
        {
            Phases.CurrentPhase.NextPhase();
        }

    }

}
