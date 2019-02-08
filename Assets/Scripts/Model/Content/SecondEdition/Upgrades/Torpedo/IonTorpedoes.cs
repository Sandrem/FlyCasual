﻿using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonTorpedoes : GenericSpecialWeapon
    {
        public IonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Torpedoes",
                UpgradeType.Torpedo,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.IonDamageAbility),
                seImageNumber: 34
            );
        }        
    }
}