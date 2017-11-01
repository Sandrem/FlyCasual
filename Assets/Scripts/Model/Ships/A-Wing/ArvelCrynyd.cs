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

                IsUnique = true;
            }

            public override bool CanAttackBumpedTarget(GenericShip defender)
            {
                return true;
            }
        }
    }
}
