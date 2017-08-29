using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public interface IModifyPilotSkill
    {
        void ModifyPilotSkill(ref int pilotSkill);
    }

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
        public int Hull { get; protected set; }
        public int Shields { get; protected set; }
        public int Cost { get; protected set; }

        private int maxHull;
        public int MaxHull
        {
            get
            {
                int result = maxHull;
                if (AfterGetMaxHull != null) AfterGetMaxHull(ref result);
                return Mathf.Max(result, 1);
            }
            protected set
            {
                maxHull = Mathf.Max(value, 1);
            }
        }

        private int maxShields;
        public int MaxShields
        {
            get
            {
                int result = maxShields;
                if (AfterGetMaxShields != null) AfterGetMaxShields(ref result);
                return Mathf.Max(result, 0);
            }
            protected set
            {
                maxShields = Mathf.Max(value, 0);
            }
        }

        protected List<IModifyPilotSkill> PilotSkillModifiers;

        private int pilotSkill;
        public int PilotSkill
        {
            get
            {
                int result = pilotSkill;
                if (PilotSkillModifiers.Count > 0) PilotSkillModifiers[0].ModifyPilotSkill(ref result);
                
                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            protected set
            {
                value = Mathf.Clamp(value, 0, 12);
                pilotSkill = value;
            }
        }

        public void AddPilotSkillModifier(IModifyPilotSkill modifier)
        {
            PilotSkillModifiers.Insert(0, modifier);
            Roster.UpdateShipStats(this);
        }

        public void RemovePilotSkillModifier(IModifyPilotSkill modifier)
        {
            PilotSkillModifiers.Remove(modifier);
            Roster.UpdateShipStats(this);
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
            BuiltInSlots = new Dictionary<Upgrade.UpgradeType, int>();
            InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeType, Upgrade.GenericUpgrade>>();
            PilotSkillModifiers = new List<IModifyPilotSkill>();

            AddCoreUpgradeSlots();
        }

        public void InitializeGenericShip(Players.PlayerNo playerNo, int shipId, Vector3 position, List<string> upgrades)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;

            AddBuiltInActions();

            StartingPosition = position;
            CreateModel(StartingPosition);

            InitializeShip();
            InitializePilot();


            foreach (var upgrade in upgrades)
            {
                InstallUpgrade(upgrade);
            }

            InfoPanel = Roster.CreateRosterInfo(this);
            Roster.UpdateUpgradesPanel(this, this.InfoPanel);

        }

        public virtual void InitializeShip()
        {
            Shields = MaxShields;
            Hull = MaxHull;

            ArcInfo = new Arcs.GenericArc(this);
            PrimaryWeapon = new PrimaryWeaponClass(this);
        }

        public virtual void InitializePilot()
        {
            SetShipInstertImage();
        }

        // STAT MODIFICATIONS

        public void ChangeAgilityBy(int value)
        {
            Agility += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeHullBy(int value)
        {
            Hull += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeShieldBy(int value)
        {
            Shields += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

    }

}
