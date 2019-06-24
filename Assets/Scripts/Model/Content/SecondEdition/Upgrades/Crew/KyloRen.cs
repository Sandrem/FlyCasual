using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Conditions;

namespace UpgradesList.SecondEdition
{
    public class KyloRen : GenericUpgrade
    {
        public KyloRen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kylo Ren",
                UpgradeType.Crew,
                cost: 11,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.KyloRenCrewAbility),
                addForce: 1
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/f60322a1f5ace7e45f6c7e0fa0200705.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenCrewAbility : Abilities.FirstEdition.KyloRenCrewAbility
    {
        protected override bool IsActionAvailbale()
        {
            return HostShip.State.Force > 0;
        }

        protected override void SpendExtra()
        {
            HostShip.State.Force--;
        }
    }
}