using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class CaptainNymScum : ScurrgH6Bomber
        {
            public CaptainNymScum() : base()
            {
                PilotName = "Captain Nym";
                PilotSkill = 8;
                Cost = 30;

                IsUnique = true;

                SkinName = "Captain Nym (Scum)";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainNymScumAbiliity());
            }
        }
    }
}

namespace Abilities
{
    public class CaptainNymScumAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
