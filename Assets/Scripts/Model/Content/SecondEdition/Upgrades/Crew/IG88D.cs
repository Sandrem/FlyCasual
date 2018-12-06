using Ship;
using Upgrade;
using ActionsList;
using Actions;

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
                addAction: new ActionInfo(typeof(CalculateAction)),
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
        bool addedAbility = false;

        public override void ActivateAbility() { }
        public override void ActivateAbilityForSquadBuilder()
        {
            if (HostShip.PilotAbilities.Find(ability => ability is AdvancedDroidBrain) == null)
            {
                HostShip.PilotAbilities.Add(new AdvancedDroidBrain());
                addedAbility = true;
            }
        }

        public override void DeactivateAbility() { }
        public override void DeactivateAbilityForSquadBuilder()
        {
            if (addedAbility)
            {
                HostShip.PilotAbilities.RemoveAll(ability => ability is AdvancedDroidBrain);
            }
        }
    }
}