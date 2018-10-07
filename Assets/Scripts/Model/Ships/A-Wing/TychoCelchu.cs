using RuleSets;
using System.Collections.Generic;

namespace Ship
{
    namespace AWing
    {
        public class TychoCelchu : AWing, ISecondEditionPilot
        {
            public TychoCelchu() : base()
            {
                PilotName = "Tycho Celchu";
                PilotSkill = 8;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TychoCelchuAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 42;

                ImageUrl = "https://i.imgur.com/lzgv9da.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.FirstEditionPilotsMod) };
            }
        }
    }
}

namespace Abilities
{
    public class TychoCelchuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = false;
        }
    }
}
