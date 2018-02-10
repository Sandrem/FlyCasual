using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using System;
using SubPhases;
using DamageDeckCard;
using System.Linq;

namespace Ship
{
    namespace UpsilonShuttle
    {
        public class KyloRenUpsilon : UpsilonShuttle
        {
            public KyloRenUpsilon() : base()
            {
                PilotName = "Kylo Ren";
                PilotSkill = 6;
                Cost = 34;

                IsUnique = true;

                UpgradeBar.AddSlot(UpgradeType.Elite);

                PilotAbilities.Add(new KyloRenPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KyloRenPilotAbility: GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
