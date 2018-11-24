using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESfFighter
    {
        public class Backdraft : TIESfFighter
        {
            public Backdraft() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Backdraft\"",
                    7,
                    27,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BackdraftAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BackdraftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddBackdraftAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddBackdraftAbility;
        }

        private void AddBackdraftAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new BackdraftAbilityAction());
        }
    }
}

namespace ActionsList
{
    public class BackdraftAbilityAction : GenericAction
    {
        public BackdraftAbilityAction()
        {
            Name = DiceModificationName = "\"Backdraft\" ability";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            result = 110;
            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack && !Combat.ShotInfo.InPrimaryArc)
            {
                result = true;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Crit).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            callBack();
        }
    }

}
