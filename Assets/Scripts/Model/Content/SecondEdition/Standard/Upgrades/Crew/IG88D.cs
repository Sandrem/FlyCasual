using Ship;
using Upgrade;
using ActionsList;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class IG88D : GenericUpgrade
    {
        public IG88D() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "IG-88D",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                addAction: new ActionInfo(typeof(CalculateAction)),
                abilityType: typeof(Abilities.SecondEdition.Ig88DCrewAbility),
                seImageNumber: 132
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(368, 2),
                new Vector2(200, 200)
            );
        }        
    }
}


namespace Abilities.SecondEdition
{
    public class Ig88DCrewAbility : Abilities.SecondEdition.Ig2000Ability
    {
        bool addedAbility = false;

        public override void ActivateAbilityForSquadBuilder()
        {
            if (HostShip.PilotAbilities.Find(ability => ability is AdvancedDroidBrain) == null)
            {
                HostShip.PilotAbilities.Add(new AdvancedDroidBrain());
                addedAbility = true;
            }
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            if (addedAbility)
            {
                HostShip.PilotAbilities.RemoveAll(ability => ability is AdvancedDroidBrain);
            }
        }
    }
}