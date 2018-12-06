using Ship;
using Upgrade;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class IG88D : GenericUpgrade
    {
        public IG88D() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "IG-88D",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.Ig88DCrewAbility),
                seImageNumber: 132
            );
        }        
    }
}


namespace Abilities.SecondEdition
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