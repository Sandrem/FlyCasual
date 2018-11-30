using Upgrade;
using Ship;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class AdaptabilityIncrease : GenericDualUpgrade
    {
        public AdaptabilityIncrease() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Adaptability (+1)",
                UpgradeType.Elite,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.AdaptabilityIncreaseAbility)
            );

            // TODOREVERT
            // ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Elite/adaptability-increase.png";

            AnotherSide = typeof(AdaptabilityDecrease);
        }
    }

    public class AdaptabilityDecrease : GenericDualUpgrade
    {
        public AdaptabilityDecrease() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Adaptability (-1)",
                UpgradeType.Elite,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.AdaptabilityDecreaseAbility)
            );

            // TODOREVERT
            // ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Elite/adaptability-decrease.png";

            AnotherSide = typeof(AdaptabilityIncrease);
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AdaptabilityIncreaseAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterDualCardSideSelected += ActivateEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.State.RemovePilotSkillModifier(this);
            HostShip.OnAfterDualCardSideSelected -= ActivateEffect;
        }

        private void ActivateEffect(GenericDualUpgrade upgrade)
        {
            HostShip.State.AddPilotSkillModifier(this);
        }

        public virtual void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill++;
        }
    }

    public class AdaptabilityDecreaseAbility : AdaptabilityIncreaseAbility
    {
        public override void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill--;
        }
    }
}