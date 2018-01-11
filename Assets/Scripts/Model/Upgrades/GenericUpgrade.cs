using Abilities;
using Mods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public Ship.GenericShip Host { get; set; }

        public string Name { get; set; }
        public int Cost;
        public UpgradeType Type;

        public List<GenericAbility> UpgradeAbilities = new List<GenericAbility>();

        public bool isUnique = false;
        public bool isLimited = false;
        public bool isDiscarded = false;

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

        public Type FromMod { get; set; }

        public virtual bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return true;
        }

        public virtual bool IsAllowedForSquadBuilder()
        {
            bool result = true;

            if (IsHidden) return false;

            if (FromMod != null && !ModsManager.Mods[FromMod].IsOn) return false;

            return result;
        }

        public virtual bool IsAllowedForSquadBuilderPostCheck(RosterBuilder.SquadBuilderUpgrade upgradeHolder)
        {
            return true;
        }

        public virtual void PreAttachToShip(Ship.GenericShip host)
        {
            Host = host;
        }

        public virtual void PreDettachFromShip()
        {

        }

        // ATTACH TO SHIP

        public virtual void AttachToShip(Ship.GenericShip host)
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

            callBack();
        }

        // FLIP FACEUP

        public virtual void FlipFaceup()
        {
            isDiscarded = false;
            Roster.FlipFaceupUpgrade(Host, Name);
            ActivateAbility();

            Messages.ShowInfo(Name + " is flipped face up");
        }

    }

}
