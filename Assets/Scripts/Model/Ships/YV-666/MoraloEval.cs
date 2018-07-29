using Arcs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YV666
    {
        public class MoraloEval : YV666
        {
            public MoraloEval() : base()
            {
                PilotName = "Moralo Eval";
                PilotSkill = 6;
                Cost = 34;

                IsUnique = true;

                SkinName = "Crimson";

                PilotAbilities.Add(new Abilities.MoraloEvalAbility());
            }
        }
    }
}

namespace Abilities
{
    public class MoraloEvalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ToggleAbility(true);
        }

        public override void DeactivateAbility()
        {
            ToggleAbility(false);
        }

        private void ToggleAbility(bool isActive)
        {
            foreach (GenericArc arc in HostShip.ArcInfo.Arcs)
            {
                if (arc is OutOfArc) continue;

                arc.ShotPermissions.CanShootCannon = isActive;
            }
        }

    }
}
