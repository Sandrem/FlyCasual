using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class CaptainNymRebel : ScurrgH6Bomber
        {
            public CaptainNymRebel() : base()
            {
                PilotName = "Captain Nym";
                PilotSkill = 8;
                Cost = 30;

                IsUnique = true;

                faction = Faction.Rebel;
                SkinName = "Captain Nym (Rebel)";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainNymRebelAbiliity());
            }
        }
    }
}

namespace Abilities
{
    public class CaptainNymRebelAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
