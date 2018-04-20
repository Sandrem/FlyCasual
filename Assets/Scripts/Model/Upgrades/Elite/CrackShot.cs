using Upgrade;
using Ship;
using Abilities;
using System;
using Tokens;
using UnityEngine;

namespace UpgradesList
{

    public class CrackShot : GenericUpgrade
    {

        public CrackShot() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Crack Shot";
            Cost = 1;

            AvatarOffset = new Vector2(43, 1);

            UpgradeAbilities.Add(new CrackShotAbility());
        }
    }
}

namespace Abilities
{
    public class CrackShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableCompareResultsEffectsList += CrackShotDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableCompareResultsEffectsList -= CrackShotDiceModification;
        }

        private void CrackShotDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.CrackShotDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = this.HostUpgrade
            };
            host.AddAvailableCompareResultsEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class CrackShotDiceModification : GenericAction
    {

        public CrackShotDiceModification()
        {
            Name = EffectName = "Crack Shot";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Combat.ShotInfo.InArc)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Source.TryDiscard(callBack);
        }

    }

}