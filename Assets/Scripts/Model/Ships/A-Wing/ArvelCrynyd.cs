namespace Ship
{
    namespace AWing
    {
        public class ArvelCrynyd : AWing
        {
            public ArvelCrynyd() : base()
            {
                PilotName = "Arvel Crynyd";
                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/e/e3/Arvel_Crynyd.png";
                PilotSkill = 6;
                Cost = 23;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal -= Rules.Collision.CanPerformAttack;
                RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += ArvelCrynydCanPerformAttack;                
            }

            private void ArvelCrynydCanPerformAttack(ref bool result, GenericShip attacker, GenericShip defender)
            {
                if (attacker is ArvelCrynyd) result = true;
                else Rules.Collision.CanPerformAttack(ref result, attacker, defender);
            }
        }
    }
}
