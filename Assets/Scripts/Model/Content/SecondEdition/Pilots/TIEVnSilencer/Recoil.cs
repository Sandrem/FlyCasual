﻿using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Recoil : TIEVnSilencer
        {
            public Recoil() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Recoil\"",
                    4,
                    57,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RecoilAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ab11858b2b9ac5c8bbfb2dc21023ba34.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RecoilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBullseyeArcCheck += CheckAbility;   
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBullseyeArcCheck -= CheckAbility;
        }

        private void CheckAbility(GenericShip anotherShip, ref bool isInBullseyeArc)
        {
            if (isInBullseyeArc) return;

            if (!HostShip.IsStressed) return;
            if (!HostShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.Front)) return;
            if (HostShip.SectorsInfo.RangeToShipBySector(anotherShip, Arcs.ArcType.Front) > 1) return;

            isInBullseyeArc = true;            
        }
    }
}