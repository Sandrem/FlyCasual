using Ship;

namespace RulesList
{
    public class TargetIsLegalForShotRule
    {
        //EVENTS
        public static event GenericShip.EventHandler2Ships OnCheckTargetIsLegal;

        public bool IsLegal(bool isSilent = false)
        {
            bool result = true;
            result = Selection.ThisShip.CallCanPerformAttack(result, null, isSilent);

            if (result) OnCheckTargetIsLegal(ref result, Selection.ThisShip, Selection.AnotherShip);
            return result;
        }
    }
}
