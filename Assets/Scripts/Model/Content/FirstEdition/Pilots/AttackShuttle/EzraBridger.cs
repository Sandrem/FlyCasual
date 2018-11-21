using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AttackShuttle
    {
        public class EzraBridger : AttackShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    4,
                    20,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.EzraBridgerPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class EzraBridgerPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddEzraBridgerPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddEzraBridgerPilotAbility;
        }


        private void AddEzraBridgerPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new EzraBridgerAction());
        }

        private class EzraBridgerAction : ActionsList.GenericAction
        {
            public EzraBridgerAction()
            {
                Name = DiceModificationName = "Ezra Bridger's ability";

                IsTurnsAllFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    result = true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    {
                        if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                    }
                }

                return result;
            }
        }

    }
}