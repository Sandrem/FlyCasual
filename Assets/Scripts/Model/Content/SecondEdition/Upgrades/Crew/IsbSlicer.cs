using Ship;
using Upgrade;
using UnityEngine;
using BoardTools;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class IsbSlicer : GenericUpgrade
    {
        public IsbSlicer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "ISB Slicer",
                UpgradeType.Crew,
                cost: 3,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.IsbSlicerAbility),
                seImageNumber: 118
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IsbSlicerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.BeforeRemovingTokenInEndPhaseGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.BeforeRemovingTokenInEndPhaseGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, GenericToken token, ref bool mustBeRemoved)
        {
            if (token is JamToken && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range == 1 || distInfo.Range == 2)
                {
                    Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Jam token is not removed from " + ship.PilotInfo.PilotName);
                    mustBeRemoved = false;
                }
            }
        }
    }
}
