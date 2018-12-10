using Upgrade;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class BB8 : GenericUpgrade
    {
        public BB8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BB-8",
                UpgradeType.Astromech,
                charges: 2,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.BB8Ability)
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/13/fe/13fe41a3-58df-41a7-ba97-38aed4a6c1fe/swz25_bb-8_a1.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before you execute a blue maneuver, you may spend 1 charge to perform a barrel roll or boost action.
    public class BB8Ability : BBAstromechAbility
    {
        public BB8Ability()
        {
            AbilityActions = new List<GenericAction> { new BarrelRollAction(), new BoostAction() };
        }
    }
}