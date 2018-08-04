using Abilities;
using Abilities.SecondEdition;
using ActionsList;
using RuleSets;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class IG88D : GenericUpgrade, ISecondEditionUpgrade
    {
        public IG88D() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "IG-88D";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(44, 2);

            UpgradeAbilities.Add(new Ig2000Ability());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;
            UpgradeAbilities.Add(new Ig88DCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
{
    public class Ig88DCrewAbility : GenericAbility
    {
        bool addedAction = false;
        bool addedAbility = false;

        public override void ActivateAbility() { }
        public override void ActivateAbilityForSquadBuilder()
        {
            if (!HostShip.ActionBar.HasAction(typeof(CalculateAction)))
            {
                HostShip.ActionBar.AddGrantedAction(new CalculateAction(), HostUpgrade);
                addedAction = true;
            }
            if (HostShip.PilotAbilities.Find(ability => ability is AdvancedDroidBrain) == null)
            {
                HostShip.PilotAbilities.Add(new AdvancedDroidBrain());
                addedAbility = true;
            }
        }

        public override void DeactivateAbility() { }
        public override void DeactivateAbilityForSquadBuilder()
        {
            if (addedAction)
            {
                HostShip.ActionBar.RemoveGrantedAction(typeof(CalculateAction), HostUpgrade);
            }
            if (addedAbility)
            {
                HostShip.PilotAbilities.RemoveAll(ability => ability is AdvancedDroidBrain);
            }
        }
    }
}