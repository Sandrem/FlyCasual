using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Sense : GenericUpgrade
    {
        public Sense() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sense",
                UpgradeType.ForcePower,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.SenseAbility),
                seImageNumber: 21
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SenseAbility : Abilities.FirstEdition.IntelligenceAgentAbility
    {
        protected override int MinRange { get { return 0; } }
        protected override int MaxRange { get { return (HostShip.State.Force > 0) ? 3 : 1; } }

        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += RegisterAbilityTriggerByShip;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= RegisterAbilityTriggerByShip;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectTargetForAbility);
            }
        }

        private void RegisterAbilityTriggerByShip(GenericShip ship, ref bool flag)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0) flag = true;
        }

        protected override void SeeAssignedManuver()
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, TargetShip);
            if (distInfo.Range > 1) { HostShip.State.Force--; }

            base.SeeAssignedManuver();
        }
    }
}