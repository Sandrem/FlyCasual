
using RuleSets;
using UnityEngine;

namespace RulesList
{
    public class DistanceBonusRule
    {

        public void CheckAttackDistanceBonus(ref int result)
        {
            if (Combat.ShotInfo.Range <= 1 && RuleSet.Instance.WeaponHasRangeBonus())
            {
                Messages.ShowInfo("Distance bonus: +1 attack die");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            if (Combat.ShotInfo.Range == 3 && RuleSet.Instance.WeaponHasRangeBonus())
            {
                Messages.ShowInfo("Distance bonus: +1 defence die");
                result++;
            }
        }

    }
}
