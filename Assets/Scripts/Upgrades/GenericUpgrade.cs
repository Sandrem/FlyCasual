using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{
    public enum UpgradeSlot
    {
        Elite,
        Astromech,
        Torpedoes,
        Missles,
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
        protected GameManagerScript Game;

        protected Ship.GenericShip Host;

        public int Cost;
        public UpgradeSlot Type;
        public bool isUnique = false;
        public bool Limited = false;
        public bool isFlippedDown = false;
        public string Name;
        public string ShortName;
        //public bool FactionRestriction
        //public bool SizeRestriction
        //public bool ShipTypeRestriction
        //public bool PilotLevelRestriction

        public GenericUpgrade(Ship.GenericShip host) {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Host = host;
        }

    }

}
