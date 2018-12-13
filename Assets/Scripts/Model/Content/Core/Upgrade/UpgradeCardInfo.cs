using Abilities;
using Actions;
using ActionsList;
using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Upgrade
{
    public class UpgradeCardInfo
    {
        private GenericUpgrade HostUpgrade;
        private GenericShip HostShip;

        public string Name { get; private set; }
        public List<UpgradeType> UpgradeTypes { get; private set; }
        public int Cost { get; set; }
        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }
        public bool FeIsLimitedPerShip { get; private set; }
        public List<Type> AbilityTypes { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }
        public int SEImageNumber { get; private set; }
        public UpgradeCardRestrictions Restrictions { get; private set; }
        public SpecialWeaponInfo WeaponInfo { get; private set; }
        public ActionInfo AddAction { get; private set; }
        public LinkedActionInfo AddActionLink { get; private set; }
        public List<UpgradeSlot> AddedSlots { get; private set; }
        public List<UpgradeType> ForbiddenSlots { get; private set; }
        public Dictionary<UpgradeType, int> CostReductionByType { get; private set; }
        public int AddHull { get; private set; }
        public int AddShields { get; private set; }
        public int AddForce { get; private set; }
        public ShipArcInfo AddArc { get; private set; }

        public UpgradeCardInfo(
            string name,
            UpgradeType type = UpgradeType.None,
            List<UpgradeType> types = null,
            int cost = 0,
            bool isLimited = false,
            int limited = 0,
            Type abilityType = null,
            UpgradeCardRestriction restriction = null,
            UpgradeCardRestrictions restrictions = null,
            int charges = 0,
            bool regensCharges = false,
            int seImageNumber = 0,
            SpecialWeaponInfo weaponInfo = null,
            ShipArcInfo addArc = null,
            ActionInfo addAction = null,
            LinkedActionInfo addActionLink = null,
            UpgradeSlot addSlot = null,
            List<UpgradeSlot> addSlots = null,
            bool feIsLimitedPerShip = false,
            UpgradeType forbidSlot = UpgradeType.None,
            List<UpgradeType> forbidSlots = null,
            Dictionary<UpgradeType, int> costReductionByType = null,
            int addShields = 0,
            int addHull = 0,
            int addForce = 0
        )
        {
            Name = name;
            Cost = cost;
            Charges = charges;
            RegensCharges = regensCharges;
            SEImageNumber = seImageNumber;
            WeaponInfo = weaponInfo;
            AddAction = addAction;
            AddActionLink = addActionLink;

            AbilityTypes = new List<Type>();
            if (abilityType != null) AbilityTypes.Add(abilityType);

            Limited = (isLimited) ? 1 : 0;
            if (limited != 0) Limited = limited;

            FeIsLimitedPerShip = feIsLimitedPerShip;

            UpgradeTypes = new List<UpgradeType>();
            if (type != UpgradeType.None) UpgradeTypes.Add(type);
            if (types != null) UpgradeTypes.AddRange(types);

            Restrictions = new UpgradeCardRestrictions();
            if (restriction != null) Restrictions = new UpgradeCardRestrictions(restriction);
            if (restrictions != null) Restrictions = restrictions;

            AddedSlots = new List<UpgradeSlot>();
            if (addSlot != null) AddedSlots.Add(addSlot);
            if (addSlots != null) AddedSlots.AddRange(addSlots);

            ForbiddenSlots = new List<UpgradeType>();
            if (forbidSlot != UpgradeType.None) ForbiddenSlots.Add(forbidSlot);
            if (forbidSlots != null) ForbiddenSlots.AddRange(forbidSlots);

            CostReductionByType = new Dictionary<UpgradeType, int>();
            if (costReductionByType != null) CostReductionByType = costReductionByType;

            AddHull = addHull;
            AddShields = addShields;
            AddForce = addForce;

            AddArc = addArc;
        }

        public bool HasType(UpgradeType upgradeType)
        {
            return UpgradeTypes.Contains(upgradeType);
        }

        public void InstallToShip(GenericUpgrade hostUpgrade)
        {
            HostUpgrade = hostUpgrade;
            HostShip = hostUpgrade.HostShip;

            AddStats();
            AddArcs();
            AddSlots();
            AddActions();
            AddAbilities();
        }

        public void RemoveFromShip()
        {
            RemoveStats();
            RemoveArcs();
            RemoveSlots();
            RemoveActions();
            RemoveAbilities();
        }

        private void AddStats()
        {
            HostShip.ShipInfo.Hull += AddHull;
            HostShip.ShipInfo.Shields += AddShields;
            HostShip.PilotInfo.Force += AddForce;
        }

        private void AddArcs()
        {
            if (AddArc != null) HostShip.ShipInfo.ArcInfo.Arcs.Add(AddArc);
        }

        private void AddSlots()
        {
            AddedSlots.ForEach(slot => {
                slot.GrantedBy = this;
                HostUpgrade.HostShip.UpgradeBar.AddSlot(slot);
            });

            ForbiddenSlots.ForEach(type =>
            {
                HostUpgrade.HostShip.UpgradeBar.ForbidSlots(type);
            });

            foreach (var item in CostReductionByType)
            {
                HostUpgrade.HostShip.UpgradeBar.CostReduceByType(item.Key, item.Value);
            }
        }

        private void AddActions()
        {
            if (AddAction != null)
            {
                /*GenericAction addedAction = (GenericAction)Activator.CreateInstance(AddAction.ActionType);
                addedAction.IsRed = (AddAction.Color == ActionColor.Red);
                addedAction.HostShip = HostUpgrade.HostShip;

                HostUpgrade.HostShip.ActionBar.AddGrantedAction(addedAction, HostUpgrade);*/

                HostShip.ShipInfo.ActionIcons.AddActions(new ActionInfo(AddAction.ActionType, AddAction.Color));
            }

            if (AddActionLink != null)
            {
                GenericAction linkedAction = (GenericAction)Activator.CreateInstance(AddActionLink.ActionLinkedType);
                linkedAction.IsRed = (AddActionLink.LinkedColor == ActionColor.Red);
                linkedAction.HostShip = HostUpgrade.HostShip;

                HostUpgrade.HostShip.ActionBar.AddActionLink(AddActionLink.ActionType, linkedAction);
            }
        }

        private void RemoveArcs()
        {
            if (AddArc != null) HostShip.ShipInfo.ArcInfo.Arcs.Remove(AddArc);
        }

        private void AddAbilities()
        {
            foreach (Type abilityType in AbilityTypes)
            {
                HostUpgrade.UpgradeAbilities.Add((GenericAbility)Activator.CreateInstance(abilityType));
            }

            foreach (GenericAbility ability in HostUpgrade.UpgradeAbilities)
            {
                ability.InitializeForSquadBuilder(HostUpgrade);
            }
        }

        private void RemoveStats()
        {
            HostShip.ShipInfo.Hull -= AddHull;
            HostShip.ShipInfo.Shields -= AddShields;
            HostShip.PilotInfo.Force -= AddForce;
        }

        private void RemoveSlots()
        {
            AddedSlots.ForEach(slot => HostUpgrade.HostShip.UpgradeBar.RemoveSlot(slot.Type, this));

            ForbiddenSlots.ForEach(type => HostUpgrade.HostShip.UpgradeBar.AllowSlots(type));

            foreach (var item in CostReductionByType)
            {
                HostUpgrade.HostShip.UpgradeBar.CostReduceByType(item.Key, -item.Value);
            }
        }

        private void RemoveActions()
        {
            if (AddAction != null)
            {
                ActionInfo addedAction = HostShip.ShipInfo.ActionIcons.Actions.First(a => a.ActionType == AddAction.ActionType && a.Color == AddAction.Color);
                HostShip.ShipInfo.ActionIcons.Actions.Remove(addedAction);

                //HostUpgrade.HostShip.ActionBar.RemoveGrantedAction(AddAction.ActionType, HostUpgrade);
            }

            if (AddActionLink != null)
            {
                HostUpgrade.HostShip.ActionBar.RemoveActionLink(AddActionLink.ActionType, AddActionLink.ActionLinkedType);
            }
        }

        private void RemoveAbilities()
        {
            foreach (GenericAbility ability in HostUpgrade.UpgradeAbilities)
            {
                ability.DeactivateAbilityForSquadBuilder();
            }
            HostUpgrade.UpgradeAbilities.Clear();
        }
    }
}
