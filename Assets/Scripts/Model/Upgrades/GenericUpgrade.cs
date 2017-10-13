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
        public Ship.GenericShip Host { get; set; }

        public int Cost;
        public UpgradeType Type;
        public bool isUnique = false;
        public bool Limited = false;
        public bool isDiscarded = false;
        public string Name { get; set; }

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
        //public bool FactionRestriction
        //public bool SizeRestriction
        //public bool ShipTypeRestriction
        //public bool PilotLevelRestriction

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

        public virtual void Discard()
        {
            isDiscarded = true;
            Roster.DiscardUpgrade(Host, Name);
        }

    }

}
