using BoardTools;
using Ship;
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
                UpgradeType.Force,
                cost: 6,
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
            HostShip.OnSystemsAbilityActivation += RegisterAbilityTriggerByShip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterAbilityTriggerByShip;
        }

        private void RegisterAbilityTriggerByShip(GenericShip ship)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectTargetForAbility);
            }
            else
            {
                Messages.ShowInfo("No ships is in range of Sense.");
            }
        }

        protected override void SeeAssignedManuver()
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, TargetShip);
            if (distInfo.Range > 1) { HostShip.State.Force--; }

            base.SeeAssignedManuver();
        }
    }
}