using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        protected GameManagerScript Game;

        public int ShipId { get; set; }
        public Players.GenericPlayer Owner { get; set; }

        public string Type { get; set; }

        public Faction faction;
        public List<Faction> factions = new List<Faction>();
        
        public string PilotName { get;  set; }
        public bool isUnique = false;

        public int Firepower { get; set; }
        public int MaxHull { get; set; }
        public int Hull { get; set; }
        public int MaxShields { get; set; }
        public int Shields { get; set; }
        public int Cost { get; set; }

        private int pilotSkill;
        public int PilotSkill
        {
            get
            {
                int result = pilotSkill;
                if (AfterGetPilotSkill!=null) AfterGetPilotSkill(ref result);
                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            set
            {
                value = Mathf.Clamp(value, 0, 12);
                pilotSkill = value;
            }
        }

        private int agility;
        public int Agility
        {
            get
            {
                int result = agility;
                if (AfterGetAgility != null) AfterGetAgility(ref result);
                result = Mathf.Max(result, 0);
                return result;
            }
            set
            {
                value = Mathf.Max(value, 0);
                agility = value;
            }
        }

        public GameObject Model { get; set; }
        public GameObject InfoPanel { get; set;  }

        public GenericShip()
        {
            AddCoreUpgradeSlots();
        }

        public void InitializeGenericShip(Players.PlayerNo playerNo, int shipId, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;

            AddBuiltInActions();

            StartingPosition = position;
            CreateModel(StartingPosition);
        }

        public virtual void InitializeShip()
        {
            Shields = MaxShields;
            Hull = MaxHull;
        }

        public virtual void InitializePilot()
        {
            SetShipInstertImage();
        }

        // STAT MODIFICATIONS

        public void ChangeAgilityBy(int value)
        {
            Agility += value;
            AfterStatsAreChanged(this);
        }

    }

}
