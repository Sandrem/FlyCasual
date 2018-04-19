using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using ActionsList;
using Upgrade;

namespace Ship
{
    public enum WingsPositions { Opened, Closed };

    public interface IMovableWings
    {
        WingsPositions CurrentWingsPosition { get; set; }
    }

    public partial class GenericShip
    {
        public Dictionary<Faction, Type> IconicPilots;

        public event EventHandlerShip OnDocked;
        public event EventHandlerShip OnUndocked;

        public event EventHandlerShip OnShipIsPlaced;

        public event EventHandlerActionInt OnAiGetDiceModificationPriority;

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

        public List<Type> RequiredMods { get; set; }

        public event EventHandler OnDiscardUpgrade;
        public event EventHandlerUpgrade OnAfterDiscardUpgrade;

        public event EventHandler OnFlipFaceUpUpgrade;
        public event EventHandlerUpgrade OnAfterFlipFaceUpUpgrade;

        public event EventHandlerDualUpgrade OnAfterDualCardSideSelected;

        public void CallOnShipIsPlaced(Action callback)
        {
            if (OnShipIsPlaced != null) OnShipIsPlaced(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsPlaced, callback);
        }

        public void CallDiscardUpgrade(Action callBack)
        {
            if (OnDiscardUpgrade != null) OnDiscardUpgrade();

            Triggers.ResolveTriggers(TriggerTypes.OnDiscard, callBack);
        }

        public void CallFlipFaceUpUpgrade(Action callBack)
        {
            if (OnFlipFaceUpUpgrade != null) OnFlipFaceUpUpgrade();

            Triggers.ResolveTriggers(TriggerTypes.OnFlipFaceUp, callBack);
        }

        public void CallAfterDiscardUpgrade(GenericUpgrade discardedUpgrade, Action callBack)
        {
            if (OnAfterDiscardUpgrade != null) OnAfterDiscardUpgrade(discardedUpgrade);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterDiscard, callBack);
        }

        public void CallAfterFlipFaceUpUpgrade(GenericUpgrade flippedFaceUpUpgrade, Action callBack)
        {
            if (OnAfterFlipFaceUpUpgrade != null) OnAfterFlipFaceUpUpgrade(flippedFaceUpUpgrade);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterFlipFaceUp, callBack);
        }

        public void CallOnAfterDualUpgradeSideSelected(GenericDualUpgrade upgrade)
        {
            if (OnAfterDualCardSideSelected != null) OnAfterDualCardSideSelected(upgrade);
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

            if (RequiredMods.Count != 0)
            {
                foreach (var modType in RequiredMods)
                {
                    if (!ModsManager.Mods[modType].IsOn) return false;
                }
            }

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

        // AI

        public void CallOnAiGetDiceModificationPriority(GenericAction diceModification, ref int priority)
        {
            if (OnAiGetDiceModificationPriority != null) OnAiGetDiceModificationPriority(diceModification, ref priority);
        }
    }

}
