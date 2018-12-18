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

        public Faction SubFaction
        {
            get
            {
                if (ShipInfo.SubFaction != Faction.None)
                {
                    return ShipInfo.SubFaction;
                }
                else
                {
                    return Faction;
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

        public ArcsHolder ArcsInfo { get; protected set; }
        public SectorsHolder SectorsInfo { get; set; }

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

                if (string.IsNullOrEmpty(OldShipTypeName))
                {
                    return Tools.Canonicalize(ShipInfo.ShipName);
                }
                else
                {
                    return Tools.Canonicalize(OldShipTypeName);
                }
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

            foreach (ShipArcInfo arcInfo in ShipInfo.ArcInfo.Arcs)
            {
                if (arcInfo.Firepower != -1) PrimaryWeapons.Add(new PrimaryWeaponClass(this, arcInfo));
            }

            Damage = new Damage(this);
            ActionBar.Initialize();
        }

        public void InitializeShipModel()
        {
            CreateModel(StartingPosition);
            InitializeSectors();
            InitializeShipBaseArc();

            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());

            SetShipInsertImage();
            SetShipSkin();
        }

        public void InitializeSectors()
        {
            SectorsInfo = new SectorsHolder(this);
        }

        public void InitializeShipBaseArc()
        {
            ArcsInfo = new ArcsHolder(this);
            foreach (ShipArcInfo arc in ShipInfo.ArcInfo.Arcs)
            {
                switch (arc.ArcType)
                {
                    case ArcType.Front:
                        ArcsInfo.Arcs.Add(new ArcPrimary(ShipBase));
                        break;
                    case ArcType.Rear:
                        ArcsInfo.Arcs.Add(new ArcRear(ShipBase));
                        break;
                    case ArcType.FullFront:
                        ArcsInfo.Arcs.Add(new ArcFullFront(ShipBase));
                        break;
                    case ArcType.SingleTurret:
                        ArcsInfo.Arcs.Add(new ArcSingleTurret(ShipBase));
                        break;
                    case ArcType.DoubleTurret:
                        ArcsInfo.Arcs.Add(new ArcDualTurretA(ShipBase));
                        ArcsInfo.Arcs.Add(new ArcDualTurretB(ShipBase));
                        break;
                    case ArcType.Bullseye:
                        ArcsInfo.Arcs.Add(new ArcBullseye(ShipBase));
                        break;
                    case ArcType.TurretPrimaryWeapon:
                        //TODOREVERT
                        // Primary weapon can be used from outside the arc
                        break;
                    case ArcType.SpecialGhost:
                        ArcsInfo.Arcs.Add(new ArcSpecialGhost(ShipBase));
                        break;
                    default:
                        break;
                }
            }
        }

        public void InitializePilotForSquadBuilder()
        {
            InitializeSquadBuilderAbilities();
            InitializeSlots();
        }

        private void InitializeSquadBuilderAbilities()
        {
            foreach (GenericAbility shipAbility in ShipAbilities)
            {
                shipAbility.InitializeForSquadBuilder(this);
            }
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
            if (PilotInfo.AbilityType != null) PilotAbilities.Add((GenericAbility)Activator.CreateInstance(PilotInfo.AbilityType));

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

            foreach (var slot in PilotInfo.ExtraUpgrades)
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
            if (State != null) State.Agility += value;
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
