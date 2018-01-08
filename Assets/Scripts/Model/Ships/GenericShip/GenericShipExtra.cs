using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;

namespace Ship
{

    public partial class GenericShip
    {
        public string IconicPilot;

        public event EventHandlerShip OnDocked;
        public event EventHandlerShip OnUndocked;

        public GenericShip Host;

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

        public Type FromMod { get; set; }

        public event EventHandler OnDiscardUpgrade;

        public void CallDiscardUpgrade(Action callBack)
        {
            if (OnDiscardUpgrade != null) OnDiscardUpgrade();

            Triggers.ResolveTriggers(TriggerTypes.OnDiscard, callBack);
        }

        public List<GenericShip> DockedShips = new List<GenericShip>();

        public void ToggleDockedModel(GenericShip dockedShip, bool isVisible)
        {
            GetModelTransform().Find("DockedShips").transform.Find(dockedShip.Type).gameObject.SetActive(isVisible);
        }

        public void CallDocked(GenericShip host)
        {
            if (OnDocked != null) OnDocked(host);
        }

        public void CallUndocked(GenericShip host)
        {
            if (OnUndocked != null) OnUndocked(host);
        }

        public virtual bool IsAllowedForSquadBuilder()
        {
            bool result = true;

            if (IsHidden) return false;

            if (FromMod != null && !ModsManager.Mods[FromMod].IsOn) return false;

            return result;
        }

        public void CheckAITable()
        {
            if (HotacManeuverTable != null)
            {
                HotacManeuverTable.Check(this.Maneuvers);
            }
        }

        public void SetDockedName(bool isActive)
        {
            string dockedPosfix = " (Docked)";
            if (isActive)
            {
                PilotName = PilotName + dockedPosfix;
            }
            else
            {
                PilotName = PilotName.Replace(dockedPosfix, "");
            }
            Roster.UpdateShipStats(this);
        }
    }

}
