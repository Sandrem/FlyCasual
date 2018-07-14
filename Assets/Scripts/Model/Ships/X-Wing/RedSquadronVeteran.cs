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
                Cost = 46; //TODO

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Rebel;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
