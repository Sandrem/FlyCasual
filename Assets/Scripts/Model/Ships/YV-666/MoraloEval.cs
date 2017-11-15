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

                PilotAbilities.Add(new PilotAbilitiesNamespace.MoraloEvalAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class MoraloEvalAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            foreach (Arcs.ArcInfo arc in Host.ArcInfo.GetAllArcs())
            {
                arc.ShotPermissions.CanShootCannon = true;
            }
        }
    }
}
