using RuleSets;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class StormSquadronPilot : TIEAdvanced, ISecondEditionPilot
        {
            public StormSquadronPilot() : base()
            {
                PilotName = "Storm Squadron Pilot";
                PilotSkill = 4;
                Cost = 23;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Storm Squadron Ace";
                PilotSkill = 3;

                Cost = 43;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
