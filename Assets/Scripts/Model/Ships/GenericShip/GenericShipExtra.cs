using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        public string ImageUrl { get; protected set; }
        public string ManeuversImageUrl { get; protected set; }

        public string SoundShotsPath { get; protected set; }
        public int ShotsCount { get; protected set; }
        public List<string> SoundFlyPaths { get; protected set; }
    }

}
