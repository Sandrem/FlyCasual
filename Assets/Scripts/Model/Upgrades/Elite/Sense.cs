using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using BoardTools;

namespace UpgradesList
{
    public class Sense : GenericUpgrade
    {
        public Sense() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Sense";
            Cost = 10;

            ImageUrl = "https://i.imgur.com/WEduTk1.png";

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.SenseAbility());
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class SenseAbility : IntelligenceAgentAbility
        {
            protected override int MinRange { get { return 0; } }
            protected override int MaxRange { get { return (HostShip.Force > 0) ? 3 : 1; } }

            public override void ActivateAbility()
            {
                HostShip.OnSystemsAbilityActivation += RegisterAbilityTriggerByShip;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnSystemsAbilityActivation += RegisterAbilityTriggerByShip;
            }

            private void RegisterAbilityTriggerByShip(GenericShip ship)
            {
                if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectTargetForAbility);
                }
                else
                {
                    Messages.ShowInfo("No ships in range of ability");
                }
            }

            protected override void SeeAssignedManuver()
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, TargetShip);
                if (distInfo.Range > 1) { HostShip.Force--; }

                base.SeeAssignedManuver();
            }
        }
    }
}