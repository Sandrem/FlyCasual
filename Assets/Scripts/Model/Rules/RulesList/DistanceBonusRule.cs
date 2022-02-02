
using Editions;

namespace RulesList
{
    public class DistanceBonusRule
    {
        public delegate void EventHandlerRefBool(ref bool isAvailable);
        public event EventHandlerRefBool OnCheckAllowRangeOneBonus;
        public event EventHandlerRefBool OnCheckPreventRangeOneBonus;
        public event EventHandlerRefBool OnCheckAllowRangeThreeBonus;
        public event EventHandlerRefBool OnCheckPreventRangeThreeBonus;


        public void CheckAttackDistanceBonus(ref int result)
        {
            if (Edition.Current.IsWeaponHaveRangeBonus(Combat.ChosenWeapon))
            {
                if (IsRangeOneBonusActive())
                {
                    Messages.ShowInfo("Range " + Combat.ShotInfo.Range + " is applying a distance bonus of +1 attack die");
                    result++;
                }
            }
        }

        private bool IsRangeOneBonusActive()
        {
            bool isActive = Edition.Current.RuleSet.HasAttackRangeBonus(Combat.ShotInfo.Range);
            OnCheckAllowRangeOneBonus?.Invoke(ref isActive);
            OnCheckPreventRangeOneBonus?.Invoke(ref isActive);
            return isActive;
        }

        public void CheckDefenceDistanceBonus(ref int result)
        {
            if (Edition.Current.IsWeaponHaveRangeBonus(Combat.ChosenWeapon))
            {
                if (IsRangeThreeBonusActive())
                {
                    Messages.ShowInfo("Range 3 is applying a distance bonus of +1 defense die");
                    result++;
                }
            }
        }

        private bool IsRangeThreeBonusActive()
        {
            bool isActive = Combat.ShotInfo.Range == 3;
            OnCheckAllowRangeThreeBonus?.Invoke(ref isActive);
            OnCheckPreventRangeThreeBonus?.Invoke(ref isActive);
            return isActive;
        }

    }
}
