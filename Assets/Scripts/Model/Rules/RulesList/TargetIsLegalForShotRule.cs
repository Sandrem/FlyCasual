using Ship;

namespace RulesList
{
    public class TargetIsLegalForShotRule
    {
        public bool IsLegal(bool isSilent = false)
        {
            bool result = true;
            result = Selection.ThisShip.CallCanPerformAttack(result, null, isSilent);
            return result;
        }
    }
}
