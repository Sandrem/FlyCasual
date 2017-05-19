using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class GenericPhase
    {
        protected GameManagerScript Game;

        public string Name;

        public virtual void StartPhase() { }
        public virtual void NextPhase() { }

    }

}