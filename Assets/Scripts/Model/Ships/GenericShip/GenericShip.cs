using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        protected GameManagerScript Game;

        public int ShipId { get; set; }

        public bool isUnique = false;

        public Faction faction;
        public List<Faction> factions = new List<Faction>();

        public string Type { get; set; }
        public string PilotName { get;  set; }
        public Players.GenericPlayer Owner { get; set; }

        public string ImageUrl { get; set; }
        public string ManeuversImageUrl { get; set; }

        public string SoundShotsPath { get; set; }
        public int ShotsCount { get; set; }
        public List<string> SoundFlyPaths = new List<string>();

        public int Firepower { get; set; }
        public int MaxHull { get; set; }
        public int Hull { get; set; }
        public int MaxShields { get; set; }
        public int Shields { get; set; }
        public int Cost { get; set; }

        private int _PilotSkill;
        public int PilotSkill
        {
            get
            {
                int result = _PilotSkill;
                if (AfterGetPilotSkill!=null) AfterGetPilotSkill(ref result);
                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            set
            {
                value = Mathf.Clamp(value, 0, 12);
                _PilotSkill = value;
            }
        }

        private int _Agility;
        public int Agility
        {
            get
            {
                int result = _Agility;
                if (AfterGetAgility != null) AfterGetAgility(ref result);
                result = Mathf.Max(result, 0);
                return result;
            }
            set
            {
                value = Mathf.Max(value, 0);
                _Agility = value;
            }
        }

        public GameObject Model { get; set; }
        public GameObject InfoPanel { get; set;  }

        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();
        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        public GenericShip()
        {
            AddUpgradeSlot(Upgrade.UpgradeSlot.Modification);
        }

        public void InitializeGenericShip(Players.PlayerNo playerNo, int shipId, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;
            StartingPosition = position;

            BuiltInActions.Add(new ActionsList.FocusAction());

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

        public void ChangeAgility(int value)
        {
            Agility += value;
            AfterStatsAreChanged(this);
        }

        //UPGRADES

        protected void AddUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            if (!BuiltInSlots.ContainsKey(slot))
            {
                BuiltInSlots.Add(slot, 1);
            }
            else
            {
                BuiltInSlots[slot]++;
            }

        }

        public void InstallUpgrade(string upgradeName)
        {
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName));

            Upgrade.UpgradeSlot slot = newUpgrade.Type;
            if (HasFreeUpgradeSlot(slot))
            {
                newUpgrade.AttachToShip(this);
                InstalledUpgrades.Add(new KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>(newUpgrade.Type, newUpgrade));
                Roster.UpdateUpgradesPanel(this, this.InfoPanel);
                Roster.OrganizeRosters();
            }
        }

        private bool HasFreeUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            bool result = false;
            if (BuiltInSlots.ContainsKey(slot))
            {
                int slotsAvailabe = BuiltInSlots[slot];
                foreach (var upgrade in InstalledUpgrades)
                {
                    if (upgrade.Key == slot) slotsAvailabe--;
                }

                if (slotsAvailabe > 0) result = true;
            }
            return result;
        }

    }

}
