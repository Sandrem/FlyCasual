using Abilities;
using RuleSets;

namespace Ship
{
    namespace VT49Decimator
    {
        public class CaptainOicunn : VT49Decimator, ISecondEditionPilot
        {
            public CaptainOicunn() : base()
            {
                PilotName = "Rear Admiral Chiraneau";
                PilotSkill = 3;
                Cost = 84;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new ZebOrreliosCrewAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not needed
            }
        }
    }
}