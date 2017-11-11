using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {

        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl ?? ImageUrls.GetImageUrl(this);
            }
            protected set
            {
                imageUrl = value;
            }
        }

        public string ManeuversImageUrl { get; protected set; }

        public string SoundShotsPath { get; protected set; }
        public int ShotsCount { get; protected set; }
        public List<string> SoundFlyPaths { get; protected set; }

        public bool IsHidden { get; set; }

        public event EventHandler OnDiscardUpgrade;

        public void CallDiscardUpgrade(Action callBack)
        {
            if (OnDiscardUpgrade != null) OnDiscardUpgrade();

            Triggers.ResolveTriggers(TriggerTypes.OnDiscard, callBack);
        }
    }

}
