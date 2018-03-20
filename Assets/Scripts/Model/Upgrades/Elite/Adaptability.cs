using Upgrade;
using System.Linq;
using Ship;
using ActionsList;
using Tokens;
using Abilities;
using SubPhases;
using System;

namespace UpgradesList
{
    public class AdaptabilityIncrease : GenericDualUpgrade
    {
        public AdaptabilityIncrease()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Adaptability (+1)";
            NameCanonical = "adaptability";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Elite/adaptability-increase.png";
            Cost = 0;

            AnotherSide = typeof(AdaptabilityDecrease);

            UpgradeAbilities.Add(new AdaptabilityIncreaseAbility());
        }
    }

    public class AdaptabilityDecrease : GenericDualUpgrade
    {
        public AdaptabilityDecrease()
        {            
            Types.Add(UpgradeType.Elite);
            Name = "Adaptability (-1)";
            NameCanonical = "adaptability";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Elite/adaptability-decrease.png";
            Cost = 0;

            AnotherSide = typeof(AdaptabilityIncrease);

            UpgradeAbilities.Add(new AdaptabilityDecreaseAbility());
        }         
    }
}

namespace Abilities
{
    public class AdaptabilityIncreaseAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterDualCardSideSelected += ActivateEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterDualCardSideSelected -= ActivateEffect;
        }

        private void ActivateEffect(GenericDualUpgrade upgrade)
        {
            HostShip.AddPilotSkillModifier(this);
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