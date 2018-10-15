using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class EzraBridger : SheathipedeShuttle, ISecondEditionPilot
        {
            public EzraBridger() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 5;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.EzraBridgerPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 42;
                MaxForce = 1;

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);

                PilotAbilities.RemoveAll(ability => ability is Abilities.EzraBridgerPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.EzraBridgerPilotAbilitySE());

                SEImageNumber = 39;
            }
        }
    }
}
