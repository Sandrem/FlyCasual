using ActionsList;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    class PerceptiveCopilot : GenericUpgrade
    {
        public PerceptiveCopilot() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Perceptive Copilot";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new ReconSpecialistAbility());
        }
    }
}
