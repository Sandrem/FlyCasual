using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        public string ImageUrl { get; set; }
        public string ManeuversImageUrl { get; set; }

        public string SoundShotsPath { get; set; }
        public int ShotsCount { get; set; }
        public List<string> SoundFlyPaths = new List<string>();
    }

}
