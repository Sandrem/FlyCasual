using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arcs;
using Abilities;
using System;
using RuleSets;
using Upgrade;

namespace Ship
{

    public interface IModifyPilotSkill
    {
        void ModifyPilotSkill(ref int pilotSkill);
    }

    public interface TIE { } //marker interface for ships that counts as "TIEs", ie. Twin Ion Engine MkII

    public partial class GenericShip : IImageHolder
    {
        public ShipCardInfo ShipInfo;
        public PilotCardInfo PilotInfo;
        public ShipDialInfo DialInfo;
        public ShipModelInfo ModelInfo;

        public Faction Faction { get { return (PilotInfo.Faction != Faction.None) ? PilotInfo.Faction : ShipInfo.DefaultShipFaction; } }

        public SubFaction SubFaction
        {
            get
            {
                if (ShipInfo.SubFaction != SubFaction.None)
                {
                    return ShipInfo.SubFaction;
                }
                else
                {
                    switch (Faction)
                    {
                        case Faction.Imperial:
                            return SubFaction.GalacticEmpire;
                        case Faction.Rebel:
                            return SubFaction.RebelAlliance;
                        case Faction.Scum:
                            return SubFaction.ScumAndVillainy;
                        default:
                            throw new NotImplementedException("Invalid faction: " + Faction.ToString());
                    }
                }
            }
        }

        public ShipStateInfo State;

        public int ShipId { get; private set; }
        public Players.GenericPlayer Owner { get; private set; }

        public string PilotName { get; set; }
        public string PilotNameShort { get; protected set; }

        public int TargetLockMinRange { get; protected set; }
        public int TargetLockMaxRange { get; protected set; }

        public void CallAfterGetMaxHull(ref int result)
        {
            if (AfterGetMaxHull != null) AfterGetMaxHull(ref result);
        }

        public GameObject Model { get; protected set; }
        public GameObject InfoPanel { get; protected set;  }

        public GenericShipBase ShipBase { get; protected set; }

        public BaseArcsType ShipBaseArcsType { get; set; }
        public ArcsHolder ArcsInfo { get; protected set; }

        public ShipUpgradeBar UpgradeBar { get; protected set; }
        public ShipActionBar ActionBar { get; protected set; }

        public TokensManager Tokens { get; protected set; }

        private string pilotNameCanonical;
        public string PilotNameCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(pilotNameCanonical)) return pilotNameCanonical;

                return Tools.Canonicalize(PilotInfo.PilotName);
            }
            set { pilotNameCanonical = value; }
        }

        private string shipTypeCanonical;
        public string ShipTypeCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(shipTypeCanonical)) return shipTypeCanonical;

                return Tools.Canonicalize(ShipInfo.ShipName);
            }
            set { shipTypeCanonical = value; }
        }

        public List<GenericAbility> PilotAbilities = new List<GenericAbility>();
        public List<GenericAbility> ShipAbilities = new List<GenericAbility>();

        public GenericShip()
        {
            IconicPilots = new Dictionary<Faction, Type>();
            RequiredMods = new List<Type>();
            Maneuvers = new Dictionary<string, Movement.MovementComplexity>();
            UpgradeBar = new ShipUpgradeBar(this);
            Tokens = new TokensManager(this);
            ActionBar = new ShipActionBar(this);

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

            InitializeState();

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

        private void InitializeState()
        {
            State = new ShipStateInfo(this);

            State.Initiative = PilotInfo.Initiative;
            State.PilotSkillModifiers = new List<IModifyPilotSkill>();

            State.Firepower = ShipInfo.Firepower;
            State.Agility = ShipInfo.Agility;
            State.HullMax = ShipInfo.Hull;
            State.ShieldsMax = ShipInfo.Shields;
            State.ShieldsCurrent = State.ShieldsMax;

            State.MaxForce = PilotInfo.Force;

            State.MaxCharges = PilotInfo.Charges;
            State.RegensCharges = PilotInfo.RegensCharges;

            foreach (var maneuver in DialInfo.PrintedDial)
            {
                Maneuvers.Add(maneuver.Key.ToString(), maneuver.Value);
            }
        }

        public virtual void InitializeShip()
        {
            InitializePilotForSquadBuilder();

            PrimaryWeapon = new PrimaryWeaponClass(this);
            Damage = new Damage(this);
            ActionBar.Initialize();
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
            if (ArcsInfo != null)
            {
                List<GenericArc> oldArcs = new List<GenericArc>(ArcsInfo.Arcs);
                foreach (var arc in oldArcs)
                {
                    arc.RemoveArc();
                }
            };

            ArcsInfo = new ArcsHolder(this);

            switch (ShipBaseArcsType)
            {
                case BaseArcsType.ArcRear:
                    ArcsInfo.Arcs.Add(new ArcRear(ShipBase));
                    break;
                case BaseArcsType.ArcSpecial180:
                    ArcsInfo.Arcs.Add(new ArcSpecial180(ShipBase));
                    break;
                case BaseArcsType.Arc360:
                    ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootPrimaryWeapon = true;
                    break;
                case BaseArcsType.ArcMobile:
                    ArcsInfo.Arcs.Add(new ArcMobile(ShipBase));
                    break;
                case BaseArcsType.ArcMobileTurret:
                    ArcMobile turretArc = new ArcMobile(ShipBase);
                    turretArc.ShotPermissions.CanShootPrimaryWeapon = false;
                    ArcsInfo.Arcs.Add(turretArc);
                    break;
                case BaseArcsType.ArcMobileOnly:
                    ArcsInfo.Arcs.Add(new ArcMobile(ShipBase));
                    DisablePrimaryFiringArc();
                    break;
                case BaseArcsType.ArcMobileDual:
                    ArcsInfo.Arcs.Add(new ArcMobileDualA(ShipBase));
                    ArcsInfo.Arcs.Add(new ArcMobileDualB(ShipBase));
                    DisablePrimaryFiringArc();
                    break;
                case BaseArcsType.ArcBullseye:
                    ArcsInfo.Arcs.Add(new ArcBullseye(ShipBase));
                    break;
                case BaseArcsType.ArcSpecialGhost:
                    ArcsInfo.Arcs.Add(new ArcSpecialGhost(ShipBase));
                    break;
                default:
                    break;
            }

            Edition.Instance.AdaptArcsToRules(this);
        }

        private void DisablePrimaryFiringArc()
        {
            ArcPrimary arcPrimary = ArcsInfo.GetArc<ArcPrimary>();
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
            State.Force = State.MaxForce;
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
            foreach (var slot in ShipInfo.UpgradeIcons.Upgrades)
            {
                UpgradeBar.AddSlot(slot);
            }
        }

        // STAT MODIFICATIONS

        public void ChangeFirepowerBy(int value)
        {
            State.Firepower += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeAgilityBy(int value)
        {
            State.Agility += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeMaxHullBy(int value)
        {
            State.HullMax += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void ChangeShieldBy(int value)
        {
            State.ShieldsCurrent += value;
            if (AfterStatsAreChanged != null) AfterStatsAreChanged(this);
        }

        public void SetTargetLockRange(int min, int max)
        {
            TargetLockMinRange = min;
            TargetLockMaxRange = max;
        }

        // CHARGES

        public void SpendCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpendCharge();
            }
        }

        public void SpendCharge()
        {
            State.Charges--;

            if (State.Charges < 0) throw new InvalidOperationException("Cannot spend charge when you have none left");
        }

        public void RemoveCharge(Action callBack)
        {
            // for now this is just an alias of SpendCharge
            SpendCharge();
            callBack();
        }

        public void RestoreCharge()
        {
            if (State.Charges < State.MaxCharges)
            {
                State.Charges++;
            }
        }

        public void SetChargesToMax()
        {
            State.Charges = State.MaxCharges;
        }

    }

}
