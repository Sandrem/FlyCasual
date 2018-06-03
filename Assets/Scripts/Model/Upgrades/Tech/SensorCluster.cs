using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Tokens;

namespace UpgradesList
{

    public class SensorCluster : GenericUpgrade
    {
        public SensorCluster() : base()
        {
            Types.Add(UpgradeType.Tech);
            Name = "Sensor Cluster";
            Cost = 2;

            UpgradeAbilities.Add(new SensorClusterAbility());
        }
    }

}

namespace Abilities
{
    public class SensorClusterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += SensorClusterActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= SensorClusterActionEffect;
        }

        private void SensorClusterActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SensorClusterActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class SensorClusterActionEffect : GenericAction
    {

        public SensorClusterActionEffect()
        {
            Name = EffectName = "Sensor Cluster";
            DiceModificationTiming = DiceModificationTimingType.Normal;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Host.Tokens.HasToken(typeof(FocusToken)) &&
                Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes &&
                Combat.DiceRollDefence.Blanks > 0 &&
                Combat.DiceRollDefence.Focuses <= 1)
            {
                result = 40;
            }

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence &&
                Combat.DiceRollDefence.Blanks > 0 &&
                Host.Tokens.HasToken(typeof(FocusToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Host.Tokens.RemoveToken(typeof(FocusToken), delegate
            {
                Combat.DiceRollDefence.ChangeOne(DieSide.Blank, DieSide.Success, false);
                callBack();
            });

        }

    }

}