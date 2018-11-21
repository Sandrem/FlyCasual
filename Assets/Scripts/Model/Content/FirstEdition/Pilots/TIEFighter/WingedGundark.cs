using BoardTools;
using Ship;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class WingedGundark : TIEFighter
        {
            public WingedGundark() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Winged Gundark\"",
                    5,
                    15,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WingedGundarkAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class WingedGundarkAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += WingedGundarkPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= WingedGundarkPilotAbility;
        }

        private void WingedGundarkPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new WingedGundarkAction());
        }

        private class WingedGundarkAction : ActionsList.GenericAction
        {

            public WingedGundarkAction()
            {
                Name = DiceModificationName = "\"Winged Gundark\"'s ability";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                    if (shotInformation.Range == 1)
                    {
                        result = true;
                    }
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 20;
                }

                return result;
            }

        }
    }
}
