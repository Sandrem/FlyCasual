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
        Cargo,
        HardPoint,
        Team,
        Tech
    }

    public class GenericUpgrade
    {
        public static GenericUpgrade CurrentUpgrade;

        public Ship.GenericShip Host { get; set; }

        public int Cost;
        public UpgradeType Type;
        public bool isUnique = false;
        public bool isLimited = false;
        public bool isDiscarded = false;
        public string Name { get; set; }

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

        public bool IsHidden;

        public GenericUpgrade()
        {

        }

        public virtual bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return true;
        }

        public virtual void AttachToShip(Ship.GenericShip host)
        {
            Host = host;
        }

        public virtual void PreAttachToShip(Ship.GenericShip host)
        {
            Host = host;
        }

        public virtual void PreDettachFromShip()
        {

        }

        public virtual void TryDiscard(Action callBack)
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

            callBack();
        }

        public virtual void FlipFaceup()
        {
            isDiscarded = false;
            Roster.FlipFaceupUpgrade(Host, Name);

            Messages.ShowInfo(Name + " is flipped face up");
        }

    }

}
