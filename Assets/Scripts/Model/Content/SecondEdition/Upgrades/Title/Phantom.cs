using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Phantom : GenericUpgrade
    {
        public Phantom() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Phantom",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(
                        typeof(Ship.SecondEdition.AttackShuttle.AttackShuttle),
                        typeof(Ship.SecondEdition.SheathipedeClassShuttle.SheathipedeClassShuttle)
                    )
                ),
                abilityType: typeof(Abilities.SecondEdition.PhantomAbility),
                seImageNumber: 106
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PhantomAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetDockingRange += ModifyDockingRange;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetDockingRange -= ModifyDockingRange;
        }

        private void ModifyDockingRange(GenericShip ship, ref Vector2 range)
        {
            range = new Vector2(range.x, Mathf.Max(range.y, 1));
        }
    }
}