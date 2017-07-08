using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        protected GameManagerScript Game;

        public int ShipId { get; private set; }
        public Players.GenericPlayer Owner { get; private set; }

        public string Type { get; protected set; }

        public Faction faction { get; protected set; }
        public List<Faction> factions { get; protected set; }
        
        public string PilotName { get; protected set; }
        public bool IsUnique { get; protected set; }

        public int Firepower { get; protected set; }
        public int MaxHull { get; protected set; }
        public int Hull { get; protected set; }
        public int MaxShields { get; protected set; }
        public int Shields { get; protected set; }
        public int Cost { get; protected set; }

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
            protected set
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
            protected set
            {
                value = Mathf.Max(value, 0);
                agility = value;
            }
        }

        public GameObject Model { get; private set; }
        public GameObject InfoPanel { get; private set;  }

        public GenericShip()
        {
            factions = new List<Faction>();
            SoundFlyPaths = new List<string> ();
            Maneuvers = new Dictionary<string, Movement.ManeuverColor>();
            BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();
            InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

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

            InitializeShip();
            InitializePilot();

            InfoPanel = Roster.CreateRosterInfo(this);
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
