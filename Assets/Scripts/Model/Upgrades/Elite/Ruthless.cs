using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using SubPhases;
using System;
using Abilities;
using RuleSets;
using ActionsList;

namespace UpgradesList
{
    public class Ruthless : GenericUpgrade
    {
        public Ruthless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Ruthless";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class RuthlessAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList += AddDiceModification;
            }

            public override void DeactivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList -= AddDiceModification;
            }

            private void AddDiceModification(GenericShip host)
            {
                GenericAction newAction = new RuthlessDiceModification
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = host,
                    Source = HostUpgrade
                };
                host.AddAvailableAction(newAction);
            }
        }
    }
}

namespace ActionsList
{
    public class RuthlessDiceModification : GenericAction
    {
        public RuthlessDiceModification()
        {
            Name = EffectName = "Ruthless";
        }

        public override void ActionEffect(Action callBack)
        {
            //TODO: Select ship
        }

        public override bool IsActionEffectAvailable()
        {
            //TODO: Checks
            return true;
        }

        public override int GetActionEffectPriority()
        {
            //TODO: AI
            return 0;
        }
    }
}
