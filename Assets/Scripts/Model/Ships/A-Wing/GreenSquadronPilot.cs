using RuleSets;

namespace Ship
{
    namespace AWing
    {
        public class GreenSquadronPilot : AWing, ISecondEditionPilot
        {
            public GreenSquadronPilot() : base()
            {
                PilotName = "Green Squadron Pilot";
                PilotSkill = 3;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Green";
            }

            public void AdaptPilotToSecondEdition()
            {
                Cost = 38; //TODO
            }
        }
    }
}
