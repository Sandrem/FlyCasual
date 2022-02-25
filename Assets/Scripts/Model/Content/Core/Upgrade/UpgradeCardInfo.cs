using Abilities;
using Actions;
using ActionsList;
using Arcs;
using Content;
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
        public UpgradeSubType SubType { get; private set; }
        public int Cost { get; set; }
        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }
        public bool FeIsLimitedPerShip { get; private set; }
        public bool IsSolitary { get; private set; }
        public bool IsStandardazed { get; private set; }
        public List<Type> AbilityTypes { get; private set; }
        public int Charges { get; private set; }
        public int RegensChargesCount { get; private set; }
        public bool CannotBeRecharged { get; private set; }
        public int SEImageNumber { get; private set; }
        public UpgradeCardRestrictions Restrictions { get; private set; }
        public SpecialWeaponInfo WeaponInfo { get; private set; }
        public List<ActionInfo> AddedActions { get; private set; }
        public List<ActionInfo> AddedPotentialActions { get; private set; }
        public List<LinkedActionInfo> AddActionLinks { get; private set; }
        public List<UpgradeSlot> AddedSlots { get; private set; }
        public List<UpgradeType> ForbiddenSlots { get; private set; }
        public Dictionary<UpgradeType, int> CostReductionByType { get; private set; }
        public int AddHull { get; private set; }
        public int AddShields { get; private set; }
        public int AddForce { get; private set; }
        public ShipArcInfo AddArc { get; private set; }
        public ArcType RemoveArc { get; private set; }
        public Type RemoteType { get; private set; }
        public List<Legality> LegalityInfo { get; private set; }

        public UpgradeCardInfo(
            string name,
            UpgradeType type = UpgradeType.None,
            List<UpgradeType> types = null,
            int cost = 0,
            bool isLimited = false,
            bool isSolitary = false,
            bool isStandardazed = false,
            int limited = 0,
            Type abilityType = null,
            UpgradeCardRestriction restriction = null,
            UpgradeCardRestrictions restrictions = null,
            int charges = 0,
            bool regensCharges = false,
            int regensChargesCount = 0,
            bool cannotBeRecharged = false,
            int seImageNumber = 0,
            SpecialWeaponInfo weaponInfo = null,
            ShipArcInfo addArc = null,
            ArcType removeArc = ArcType.None,
            ActionInfo addAction = null,
            ActionInfo addPotentialAction = null,
            List<ActionInfo> addActions = null,
            List<LinkedActionInfo> addActionLinks = null,
            LinkedActionInfo addActionLink = null,
            UpgradeSlot addSlot = null,
            List<UpgradeSlot> addSlots = null,
            bool feIsLimitedPerShip = false,
            UpgradeType forbidSlot = UpgradeType.None,
            List<UpgradeType> forbidSlots = null,
            Dictionary<UpgradeType, int> costReductionByType = null,
            int addShields = 0,
            int addHull = 0,
            int addForce = 0,
            UpgradeSubType subType = UpgradeSubType.None,
            Type remoteType = null,
            List<Legality> legalityInfo = null
        )
        {
            Name = name;
            Cost = cost;
            Charges = charges;
            CannotBeRecharged = cannotBeRecharged;
            SEImageNumber = seImageNumber;
            WeaponInfo = weaponInfo;
            IsSolitary = isSolitary;
            IsStandardazed = isStandardazed;

            AbilityTypes = new List<Type>();
            if (abilityType != null) AbilityTypes.Add(abilityType);

            Limited = (isLimited) ? 1 : 0;
            if (limited != 0) Limited = limited;

            if (regensCharges) { RegensChargesCount = 1; }
            else { RegensChargesCount = regensChargesCount; }

            FeIsLimitedPerShip = feIsLimitedPerShip;

            UpgradeTypes = new List<UpgradeType>();
            if (type != UpgradeType.None) UpgradeTypes.Add(type);
            if (types != null) UpgradeTypes.AddRange(types);

            AddActionLinks = new List<LinkedActionInfo>();
            if (addActionLink != null) AddActionLinks.Add(addActionLink);
            if (addActionLinks != null) AddActionLinks.AddRange(addActionLinks);

            Restrictions = new UpgradeCardRestrictions();
            if (restriction != null) Restrictions = new UpgradeCardRestrictions(restriction);
            if (restrictions != null) Restrictions = restrictions;

            AddedSlots = new List<UpgradeSlot>();
            if (addSlot != null) AddedSlots.Add(addSlot);
            if (addSlots != null) AddedSlots.AddRange(addSlots);

            AddedActions = new List<ActionInfo>();
            if (addAction != null) AddedActions.Add(addAction);
            if (addActions != null) AddedActions.AddRange(addActions);

            AddedPotentialActions = new List<ActionInfo>();
            if (addPotentialAction != null) AddedActions.Add(addPotentialAction);

            ForbiddenSlots = new List<UpgradeType>();
            if (forbidSlot != UpgradeType.None) ForbiddenSlots.Add(forbidSlot);
            if (forbidSlots != null) ForbiddenSlots.AddRange(forbidSlots);

            CostReductionByType = new Dictionary<UpgradeType, int>();
            if (costReductionByType != null) CostReductionByType = costReductionByType;

            AddHull = addHull;
            AddShields = addShields;
            AddForce = addForce;

            AddArc = addArc;
            RemoveArc = removeArc;

            SubType = subType;
            RemoteType = remoteType;

            LegalityInfo = legalityInfo ?? new List<Legality> { Legality.StandartLegal };
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
            ForbidArcs();
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

        public void ForbidArcs()
        {
            if (RemoveArc != ArcType.None) HostShip.ShipInfo.ArcInfo.Arcs.RemoveAll(n => n.ArcType == RemoveArc);
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
            if (AddedActions.Count > 0)
            {
                foreach (ActionInfo actionInfo in AddedActions)
                {
                    HostShip.ShipInfo.ActionIcons.AddActions(new ActionInfo(actionInfo.ActionType, actionInfo.Color, HostUpgrade));
                    if (HostShip.State != null)
                    {
                        GenericAction addedAction = (GenericAction)Activator.CreateInstance(actionInfo.ActionType);
                        addedAction.Color = actionInfo.Color;
                        addedAction.HostShip = HostUpgrade.HostShip;

                        HostUpgrade.HostShip.ActionBar.AddGrantedAction(addedAction, HostUpgrade);
                    }
                }
            }

            if (AddedPotentialActions.Count > 0)
            {
                foreach (ActionInfo actionInfo in AddedPotentialActions)
                {
                    HostShip.ShipInfo.PotentialActionIcons.AddActions(new ActionInfo(actionInfo.ActionType, actionInfo.Color, HostUpgrade));
                }
            }

            foreach (LinkedActionInfo linkedActionInfo in AddActionLinks)
            {
                GenericAction linkedAction = (GenericAction)Activator.CreateInstance(linkedActionInfo.ActionLinkedType);
                linkedAction.Color = linkedActionInfo.LinkedColor;
                linkedAction.HostShip = HostUpgrade.HostShip;

                HostUpgrade.HostShip.ActionBar.AddActionLink(linkedActionInfo.ActionType, linkedAction);
            }
        }

        private void RemoveArcs()
        {
            if (AddArc != null) HostShip.ShipInfo.ArcInfo.Arcs.Remove(AddArc);
        }

        private void AddAbilities()
        {
            HostUpgrade.UpgradeAbilities = new List<GenericAbility>();
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
            if (AddedActions.Count > 0)
            {
                foreach (ActionInfo actionInfo in AddedActions)
                {
                    ActionInfo addedAction = HostShip.ShipInfo.ActionIcons.Actions.First(a =>
                        a.ActionType == actionInfo.ActionType
                        && a.Color == actionInfo.Color
                        && a.Source == HostUpgrade
                    );
                    HostShip.ShipInfo.ActionIcons.Actions.Remove(addedAction);

                    if (HostShip.State != null) HostUpgrade.HostShip.ActionBar.RemoveGrantedAction(actionInfo.ActionType, HostUpgrade);
                }
            }

            if (AddedPotentialActions.Count > 0)
            {
                foreach (ActionInfo actionInfo in AddedPotentialActions)
                {
                    ActionInfo addedAction = HostShip.ShipInfo.PotentialActionIcons.Actions.First(a =>
                        a.ActionType == actionInfo.ActionType
                        && a.Color == actionInfo.Color
                        && a.Source == HostUpgrade
                    );
                    HostShip.ShipInfo.PotentialActionIcons.Actions.Remove(addedAction);
                }
            }

            foreach (LinkedActionInfo linkedActionInfo in AddActionLinks)
            {
                HostUpgrade.HostShip.ActionBar.RemoveActionLink(linkedActionInfo.ActionType, linkedActionInfo.ActionLinkedType);
            }
        }

        private void RemoveAbilities()
        {
            foreach (GenericAbility ability in HostUpgrade.UpgradeAbilities)
            {
                ability.DeactivateAbilityForSquadBuilder();
            }
        }

        public string GetCleanName()
        {
            string cleanName = Name;
            if (Name.Contains("(")) cleanName = Name.Substring(0, Name.LastIndexOf("(") - 1);
            return cleanName;
        }
    }
}
