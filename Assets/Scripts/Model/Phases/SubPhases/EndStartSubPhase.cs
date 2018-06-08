using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class EndStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "End start";
        }

        public override void Initialize()
        {
            Phases.Events.CallEndPhaseTrigger(EndRound);
        }

        private void EndRound()
        {
            Phases.Events.CallRoundEndTrigger(delegate {
                if (!Phases.GameIsEnded) Next();
            });
            
        }

        public override void Next()
        {
            Phases.CurrentPhase.NextPhase();
        }

    }

}
