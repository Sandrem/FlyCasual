using RuleSets;

namespace Ship
{
    namespace TIEFighter
    {
        public class EzraBridgerTIE : TIEFighter, ISecondEditionPilot
        {
            public EzraBridgerTIE() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 3;
                Cost = 32;
                MaxForce = 1;

                IsUnique = true;

                faction = Faction.Rebel;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.EzraBridgerPilotAbilitySE());

                SpecialModel = "TIE Fighter Rebel";
                SkinName = "Rebel";

                SEImageNumber = 46;
            }

            public void AdaptPilotToSecondEdition()
            {
                // nope
            }
        }
    }
}