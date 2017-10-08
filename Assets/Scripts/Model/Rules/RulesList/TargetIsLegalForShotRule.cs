using Ship;

namespace RulesList
{
    public class TargetIsLegalForShotRule
    {
        //EVENTS
        public static event GenericShip.EventHandler2Ships OnCheckTargetIsLegal;

        public bool IsLegal()
        {
            bool result = true;
            result = Selection.ThisShip.CallCanPerformAttack(result);

            if (result) OnCheckTargetIsLegal(ref result, Selection.ThisShip, Selection.AnotherShip);
            return result;
        }
    }
}
