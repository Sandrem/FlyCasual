using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class RedSquadronVeteran : XWing, ISecondEditionPilot
        {
            public RedSquadronVeteran() : base()
            {
                PilotName = "Red Squadron Veteran";
                PilotSkill = 3;
                Cost = 43;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Rebel;

                SEImageNumber = 10;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
