﻿
using UnityEngine;

namespace RulesList
{
    public class DistanceBonusRule
    {

        public void CheckAttackDistanceBonus(ref int result)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo("Distance bonus: +1 attack dice");
                result++;
            }
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            if (Combat.ShotInfo.Range == 3)
            {
                Messages.ShowInfo("Distance bonus: +1 defence dice");
                result++;
            }
        }

    }
}
