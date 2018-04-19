using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcs;
using Abilities;
using System;

namespace Ship
{

    public interface IModifyPilotSkill
    {
        void ModifyPilotSkill(ref int pilotSkill);
    }

    public interface TIE { } //marker interface for ships that counts as "TIEs", ie. Twin Ion Engine MkII

    public partial class GenericShip
    {

        public int ShipId { get; private set; }
        public Players.GenericPlayer Owner { get; private set; }

        public string Type { get; protected set; }

        public Faction faction { get; protected set; }
        public List<Faction> factions { get; protected set; }

        private SubFaction? subFaction { get; set; }
        public SubFaction SubFaction
        {
            get
            {
                if (subFaction != null)
                {
                    return subFaction.Value;
                }
                else
                {
                    switch (faction)
                    {
                        case Faction.Imperial:
                            return SubFaction.GalacticEmpire;
                        case Faction.Rebel:
                            return SubFaction.RebelAlliance;
                        case Faction.Scum:
                            return SubFaction.ScumAndVillainy;
                        default:
                            throw new NotImplementedException("Invalid faction: " + faction.ToString());
                    }
                }
            }
            set
            {
                subFaction = value;
            }
        }
        
        public string PilotName { get; protected set; }
        public bool IsUnique { get; protected set; }

        public int Firepower { get; protected set; }

        public int Hull
        {
            get { return Mathf.Max(0, MaxHull - Damage.CountAssignedDamage()); }
        }


        public int Shields { get; protected set; }
        public int Cost { get; protected set; }

        public int TargetLockMinRange { get; protected set; }
        public int TargetLockMaxRange { get; protected set; }

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
                return Mathf.Max(result, 0);
            }
            set
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
                result = Mathf.Max(result, 0);
                return result;
            }
            protected set
            {
                agility = value;
            }
        }

        public GameObject Model { get; protected set; }
        public GameObject InfoPanel { get; protected set;  }

        public BaseSize ShipBaseSize { get; protected set; }
        public GenericShipBase ShipBase { get; protected set; }

        public BaseArcsType ShipBaseArcsType { get; protected set; }
        public GenericArc ArcInfo { get; protected set; }

        public Upgrade.ShipUpgradeBar UpgradeBar { get; protected set; }
        public List<Upgrade.UpgradeType> PrintedUpgradeIcons { get; protected set; }

        public TokensManager Tokens { get; protected set; }

        private string pilotNameCanonical;
        public string PilotNameCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(pilotNameCanonical)) return pilotNameCanonical;

                return Tools.Canonicalize(PilotName);
            }
            set { pilotNameCanonical = value; }
        }

        private string shipTypeCanonical;
        public string ShipTypeCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(shipTypeCanonical)) return shipTypeCanonical;

                return Tools.Canonicalize(Type);
            }
            set { shipTypeCanonical = value; }
        }

        public List<GenericAbility> PilotAbilities = new List<GenericAbility>();

        public GenericShip()
        {
            IconicPilots = new Dictionary<Faction, Type>();
            RequiredMods = new List<Type>();
            factions = new List<Faction>();
            SoundFlyPaths = new List<string> ();
            Maneuvers = new Dictionary<string, Movement.ManeuverColor>();
            UpgradeBar = new Upgrade.ShipUpgradeBar(this);
            Tokens = new TokensManager(this);
            PrintedUpgradeIcons = new List<Upgrade.UpgradeType>();
            PilotSkillModifiers = new List<IModifyPilotSkill>();

            PrintedActions = new List<ActionsList.GenericAction>();
            PrintedActions.Add(new ActionsList.FocusAction());

            TargetLockMinRange = 1;
            TargetLockMaxRange = 3;
        }

        public void InitializeGenericShip(Players.PlayerNo playerNo, int shipId, Vector3 position)
        {
            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;

            StartingPosition = position;

            InitializeShip();
            InitializePilot();
            InitializeUpgrades();

            InfoPanel = Roster.CreateRosterInfo(this);
            Roster.UpdateUpgradesPanel(this, this.InfoPanel);
        }

        public virtual void InitializeUpgrades()
        {
            foreach (var slot in UpgradeBar.GetUpgradeSlots())
            {
                slot.TryInstallUpgrade(slot.InstalledUpgrade, this);
            }
        }

        public virtual void InitializeShip()
        {
            InitializePilotForSquadBuilder();

            Shields = MaxShields;

            PrimaryWeapon = new PrimaryWeaponClass(this);
            Damage = new AssignedDamageCards(this);

            CreateModel(StartingPosition);
            InitializeShipBaseArc();

            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());
        }

        private void InitializeShipBaseArc()
        {
            switch (ShipBaseArcsType)
            {
                case BaseArcsType.ArcDefault:
                    ArcInfo = new GenericArc(this);
                    break;
                case BaseArcsType.ArcRear:
                    ArcInfo = new ArcRear(this);
                    break;
                case BaseArcsType.ArcGhost:
                    ArcInfo = new ArcGhost(this);
                    break;
                case BaseArcsType.Arc180:
                    ArcInfo = new Arc180(this);
                    break;
                case BaseArcsType.Arc360:
                    ArcInfo = new Arc360(this);
                    break;
                case BaseArcsType.ArcMobile:
                    ArcInfo = new ArcMobile(this);
                    break;
                case BaseArcsType.ArcBullseye:
                    ArcInfo = new ArcBullseye(this);
                    break;
                default:
                    break;
            }
        }

        public void InitializePilotForSquadBuilder()
        {
            InitializeSlots();
        }

        public virtual void InitializePilot()
        {
            SetShipInsertImage();
            SetShipSkin();
            InitializePilotAbilities();
        }

        private void InitializePilotAbilities()
        {
            foreach (var pilotAbility in PilotAbilities)
            {
                pilotAbility.Initialize(this);
            }
        }

        private void InitializeSlots()
        {
            foreach (var slot in PrintedUpgradeIcons)
            {
                UpgradeBar.AddSlot(slot);
            }
        }

        // STAT MODIFICATIONS

        public void ChangeFirepowerBy(int value)
        {
            Firepower += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeAgilityBy(int value)
        {
            agility += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeMaxHullBy(int value)
        {
            MaxHull += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeShieldBy(int value)
        {
            Shields += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void SetTargetLockRange(int min, int max)
        {
            TargetLockMinRange = min;
            TargetLockMaxRange = max;
        }

    }

}
