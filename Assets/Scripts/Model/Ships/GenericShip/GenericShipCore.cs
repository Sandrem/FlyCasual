using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcs;
using Abilities;
using System;
using RuleSets;
using Tokens;

namespace Ship
{

    public interface IModifyPilotSkill
    {
        void ModifyPilotSkill(ref int pilotSkill);
    }

    public interface TIE { } //marker interface for ships that counts as "TIEs", ie. Twin Ion Engine MkII

    public partial class GenericShip : IImageHolder
    {

        public int ShipId { get; private set; }
        public Players.GenericPlayer Owner { get; private set; }

        public string Type { get; protected set; }
        public string FullType { get; protected set; }

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
        
        public string PilotName { get; set; }
        public string PilotNameShort { get; protected set; }
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

        private int maxEnergy;
        public int MaxEnergy
        {
            get
            {
                int result = maxEnergy;
                return Mathf.Max(result, 0);
            }
            set
            {
                maxEnergy = Mathf.Max(value, 0);
            }
        }

        public int Energy
        {
            get
            {
                return Tokens.CountTokensByType(typeof(Tokens.EnergyToken));
            }
        }

        public int Force
        {
            get
            {
                return this.Tokens.CountTokensByType<ForceToken>();
            }

            set
            {
                this.Tokens.RemoveAllTokensByType(typeof(ForceToken), delegate { });
                for (int i = 0; i < value; i++)
                {
                    this.Tokens.AssignCondition(typeof(ForceToken));
                }
            }
        }

        public int MaxForce { get; set; }

        protected List<IModifyPilotSkill> PilotSkillModifiers;

        private int pilotSkill;
        public int PilotSkill
        {
            get
            {
                int result = pilotSkill;
                if (PilotSkillModifiers.Count > 0)
                {
                    for (int i = PilotSkillModifiers.Count-1; i >= 0; i--)
                    {
                        PilotSkillModifiers[i].ModifyPilotSkill(ref result);
                    }
                }
                
                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            set
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

        public BaseArcsType ShipBaseArcsType { get; set; }
        public ArcsHolder ArcInfo { get; protected set; }

        public Upgrade.ShipUpgradeBar UpgradeBar { get; protected set; }
        public ShipActionBar ActionBar { get; protected set; }
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
        public List<GenericAbility> ShipAbilities = new List<GenericAbility>();

        public GenericShip()
        {
            IconicPilots = new Dictionary<Faction, Type>();
            RequiredMods = new List<Type>();
            factions = new List<Faction>();
            SoundFlyPaths = new List<string> ();
            Maneuvers = new Dictionary<string, Movement.MovementComplexity>();
            UpgradeBar = new Upgrade.ShipUpgradeBar(this);
            Tokens = new TokensManager(this);
            PrintedUpgradeIcons = new List<Upgrade.UpgradeType>();
            PilotSkillModifiers = new List<IModifyPilotSkill>();

            ActionBar = new ShipActionBar(this);
            ActionBar.AddPrintedAction(new ActionsList.FocusAction());

            TargetLockMinRange = 0;
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

            InitializeShipModel();

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
            Damage = new Damage(this);
        }

        public void InitializeShipModel()
        {
            CreateModel(StartingPosition);
            InitializeShipBaseArc();

            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());

            SetShipInsertImage();
            SetShipSkin();
        }

        public void InitializeShipBaseArc()
        {
            if (ArcInfo != null)
            {
                List<GenericArc> oldArcs = new List<GenericArc>(ArcInfo.Arcs);
                foreach (var arc in oldArcs)
                {
                    arc.RemoveArc();
                }
            };

            ArcInfo = new ArcsHolder(this);

            switch (ShipBaseArcsType)
            {
                case BaseArcsType.ArcRear:
                    ArcInfo.Arcs.Add(new ArcRear(ShipBase));
                    break;
                case BaseArcsType.ArcSpecial180:
                    ArcInfo.Arcs.Add(new ArcSpecial180(ShipBase));
                    break;
                case BaseArcsType.Arc360:
                    ArcInfo.GetArc<OutOfArc>().ShotPermissions.CanShootPrimaryWeapon = true;
                    break;
                case BaseArcsType.ArcMobile:
                    ArcInfo.Arcs.Add(new ArcMobile(ShipBase));
                    break;
                case BaseArcsType.ArcMobileTurret:
                    ArcMobile turretArc = new ArcMobile(ShipBase);
                    turretArc.ShotPermissions.CanShootPrimaryWeapon = false;
                    ArcInfo.Arcs.Add(turretArc);
                    break;
                case BaseArcsType.ArcMobileOnly:
                    ArcInfo.Arcs.Add(new ArcMobile(ShipBase));
                    DisablePrimaryFiringArc();
                    break;
                case BaseArcsType.ArcMobileDual:
                    ArcInfo.Arcs.Add(new ArcMobileDualA(ShipBase));
                    ArcInfo.Arcs.Add(new ArcMobileDualB(ShipBase));
                    DisablePrimaryFiringArc();
                    break;
                case BaseArcsType.ArcBullseye:
                    ArcInfo.Arcs.Add(new ArcBullseye(ShipBase));
                    break;
                case BaseArcsType.ArcSpecialGhost:
                    ArcInfo.Arcs.Add(new ArcSpecialGhost(ShipBase));
                    break;
                default:
                    break;
            }

            RuleSet.Instance.AdaptArcsToRules(this);
        }

        private void DisablePrimaryFiringArc()
        {
            ArcPrimary arcPrimary = ArcInfo.GetArc<ArcPrimary>();
            arcPrimary.ShotPermissions.CanShootPrimaryWeapon = false;
            arcPrimary.ShotPermissions.CanShootTurret = false;
        }

        public void InitializePilotForSquadBuilder()
        {
            InitializeSlots();
        }

        public virtual void InitializePilot()
        {
            PrepareForceInitialization();
            PrepareChargesInitialization();

            ActivateShipAbilities();
            ActivatePilotAbilities();
        }

        private void PrepareForceInitialization()
        {
            OnGameStart += InitializeForce;
        }

        private void InitializeForce()
        {
            OnGameStart -= InitializeForce;
            Force = MaxForce;
        }

        private void PrepareChargesInitialization()
        {
            OnGameStart += InitializeCharges;
        }

        private void InitializeCharges()
        {
            OnGameStart -= InitializeCharges;
            SetChargesToMax();
        }

        private void ActivateShipAbilities()
        {
            foreach (var shipAbility in ShipAbilities)
            {
                shipAbility.Initialize(this);
            }
        }

        private void ActivatePilotAbilities()
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

        // CHARGES
        // TODO: Change/Remove so that this functionality isn't duplicated between GenericShip and GenericUpgrade
        public int MaxCharges { get; set; }

        private int charges;
        public int Charges
        {
            get { return charges; }
            set
            {
                int currentTokens = Tokens.CountTokensByType(typeof(ChargeToken));
                for (int i = 0; i < currentTokens; i++)
                {
                    Tokens.RemoveCondition(typeof(ChargeToken));
                }

                charges = value;
                for (int i = 0; i < value; i++)
                {
                    Tokens.AssignCondition(typeof(ChargeToken));
                }
            }
        }

        public bool UsesCharges;
        public bool RegensCharges = false;

        public void SpendCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpendCharge();
            }
        }

        public void SpendCharge()
        {
            Charges--;

            if (Charges < 0) throw new InvalidOperationException("Cannot spend charge when you have none left");
        }

        public void RemoveCharge(Action callBack)
        {
            // for now this is just an alias of SpendCharge
            SpendCharge();
            callBack();
        }

        public void RestoreCharge()
        {
            if (Charges < MaxCharges)
            {
                Charges++;
            }
        }

        public void SetChargesToMax()
        {
            Charges = MaxCharges;
        }

    }

}
