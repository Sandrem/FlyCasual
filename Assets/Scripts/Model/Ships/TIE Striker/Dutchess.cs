using RuleSets;

namespace Ship
{
    namespace TIEStriker
    {
        public class Duchess : TIEStriker, ISecondEditionPilot
        {
            public Duchess() : base()
            {
                PilotName = "\"Duchess\"";
                PilotSkill = 5;
                Cost = 42;

                IsUnique = true;
                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public void AdaptPilotToSecondEdition()
            {
                // Unneeded.
            }
        }
    }
}