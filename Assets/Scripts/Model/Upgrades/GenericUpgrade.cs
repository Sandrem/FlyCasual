using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{
    public enum UpgradeType
    {
        Elite,
        Astromech,
        Torpedoes,
        Missiles,
        Cannon,
        Turret,
        Bomb,
        Crew,
        SalvagedAstromechs,
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
        public string ShortName;
        public string ImageUrl;
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

        public virtual void SquadBuilderEffectApply(Ship.GenericShip host)
        {

        }

        public virtual void SquadBuilderEffectRemove(Ship.GenericShip host)
        {

        }

        public virtual void AttachToShip(Ship.GenericShip host)
        {
            Host = host;
        }

        public void Discard()
        {
            isDiscarded = true;
            Roster.DiscardUpgrade(Host, ShortName);
        }

    }

}
