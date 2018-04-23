using Abilities;
using Mods;
using SquadBuilderNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Upgrade
{
    public enum UpgradeType
    {
        Elite,
        Astromech,
        Torpedo,
        Missile,
        Cannon,
        Turret,
        Bomb,
        Crew,
        SalvagedAstromech,
        System,
        Title,
        Modification,
        Illicit,
        Tech
    }

    public abstract class GenericUpgrade
    {
        public static GenericUpgrade CurrentUpgrade;

        public GenericShip Host { get; set; }
        public UpgradeSlot Slot { get; set; }

        public string Name { get; set; }
        public int Cost;
        public List<UpgradeType> Types = new List<UpgradeType>();

        public List<GenericAbility> UpgradeAbilities = new List<GenericAbility>();

        public bool isUnique = false;
        public bool isLimited = false;
        public bool isDiscarded = false;
        /**
         * If true, this upgrade is used as a multi-slot card and should not be used to count squad cost or used otherwise.
         */
        public bool isPlaceholder = false;

        private string nameCanonical;
        public string NameCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(nameCanonical)) return nameCanonical;

                return Tools.Canonicalize(Name);
            }
            set { nameCanonical = value; }
        }

        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl ?? ImageUrls.GetImageUrl(this);
            }
            set
            {
                imageUrl = value;
            }
        }

        // SQUAD BUILDER ONLY

        public bool IsHidden;

        // Set to use as avatar

        public Vector2 AvatarOffset;

        //public Type FromMod { get; set; }
        public Type FromMod { get; set; }

        public bool HasEnoughSlotsInShip(GenericShip ship)
        {
            if (Types.Count > 1)
            {
                List<UpgradeSlot> freeSlots = ship.UpgradeBar.GetFreeSlots(Types);

                foreach (var requiredSlotType in Types)
                {
                    UpgradeSlot freeSlotByType = freeSlots.FirstOrDefault(n => n.Type == requiredSlotType);
                    if (freeSlotByType != null)
                    {
                        freeSlots.Remove(freeSlotByType);
                    }
                    else
                    {
                        //No free slots for this upgrade
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual bool IsAllowedForShip(GenericShip ship)
        {
            return true;
        }

        public virtual bool IsAllowedForSquadBuilder()
        {
            bool result = true;

            if (IsHidden) return false;

            if (FromMod != null && !ModsManager.Mods[FromMod].IsOn) return false;
            //if (FromMod != null && FromMod.Count != 0 && !ModsManager.Mods[FromMod[1]].IsOn) return false;
            return result;
        }

        public virtual bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            return true;
        }

        public virtual void PreAttachToShip(GenericShip host)
        {
            Host = host;
        }

        public virtual void PreDettachFromShip()
        {

        }

        /**
         * Checks if this upgrade is of the specified type.
         * @param type the type of upgrade to test.
         * @return true if this upgrade contains the specified type and false otherwise.
         */
        public bool hasType(UpgradeType type){
            for (int i = 0; i < Types.Count; i++) {
                if (Types [i] == type) {
                    return true;
                }
            }
            return false;
        }

        /**
         * Returns the type as a string.
         * @return the name of the type.
         */
        public string getTypesAsString(){
            string name = "";
            /*
            for (int i = 0; i < Types.Count; i++) {
                UpgradeType type = Types [i];
                if (i > 0) {
                    name += " ";
                }

                switch (type) {
                    case UpgradeType.SalvagedAstromech:
                        name += "Salvaged Astromech";
                        break;
                    default:
                        name += type.ToString ();
                        break;
                }
            }
            */
            UpgradeType type = Types [0];
            switch (type) {
                case UpgradeType.SalvagedAstromech:
                    name += "Salvaged Astromech";
                    break;
                default:
                    name += type.ToString ();
                    break;
            }
            return name;
        }

        // ATTACH TO SHIP

        [Obsolete("This is in the process of being depricated, please use new template instead: UpgradeAbilities.Add();", false)]
        public virtual void AttachToShip(GenericShip host)
        {
            Host = host;
            InitializeAbility();
            ActivateAbility();
        }

        private void InitializeAbility()
        {
            foreach (var ability in UpgradeAbilities)
            {
                ability.Initialize(this);
            }
        }

        private void ActivateAbility()
        {
            foreach (var ability in UpgradeAbilities)
            {
                ability.ActivateAbility();
            }
        }

        private void DeactivateAbility()
        {
            foreach (var ability in UpgradeAbilities)
            {
                ability.DeactivateAbility();
            }
        }

        // DISCARD

        public void TryDiscard(Action callBack)
        {
            CurrentUpgrade = this;
            Host.CallDiscardUpgrade(delegate { AfterTriedDiscard(callBack); });
        }

        private void AfterTriedDiscard(Action callBack)
        {
            if (CurrentUpgrade != null)
            {
                Discard(callBack);
            }
            else
            {
                callBack();
            }
        }

        public virtual void Discard(Action callBack)
        {
            isDiscarded = true;
            Roster.DiscardUpgrade(Host, Name);
            DeactivateAbility();

            Host.CallAfterDiscardUpgrade(this, callBack);
        }

        // FLIP FACEUP

        public void TryFlipFaceUp(Action callBack)
        {
            CurrentUpgrade = this;
            Host.CallFlipFaceUpUpgrade(() => AfterTriedFlipFaceUp(callBack));
        }

        private void AfterTriedFlipFaceUp(Action callBack)
        {
            if (CurrentUpgrade != null)
            {
                FlipFaceup(callBack);
            }
            else
            {
                callBack();
            }
        }

        public virtual void FlipFaceup(Action callback)
        {
            isDiscarded = false;
            Roster.FlipFaceupUpgrade(Host, Name);
            ActivateAbility();

            Messages.ShowInfo(Name + " is flipped face up");
            Host.CallAfterFlipFaceUpUpgrade(this, callback);
        }

        public void ReplaceUpgradeBy(GenericUpgrade newUpgrade)
        {
            Roster.ReplaceUpgrade(Host, Name, newUpgrade.Name, newUpgrade.ImageUrl);

            Slot.PreInstallUpgrade(newUpgrade, Host);
            Slot.TryInstallUpgrade(newUpgrade, Host);
        }
    }

}
