using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class EzraBridger : AttackShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    3,
                    41,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility),
                    force: 1,
                    extraUpgradeIcon: UpgradeType.Force,
                    seImageNumber: 36
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EzraBridgerPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddEzraBridgerPilotAbilitySE;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddEzraBridgerPilotAbilitySE;
        }

        private void AddEzraBridgerPilotAbilitySE(GenericShip ship)
        {
            EzraBridgerActionSE newAction = new EzraBridgerActionSE() { Host = this.HostShip };
            ship.AddAvailableDiceModification(newAction);
        }

        private class EzraBridgerActionSE : ActionsList.GenericAction
        {
            public EzraBridgerActionSE()
            {
                Name = DiceModificationName = "Ezra Bridger's ability";

                TokensSpend.Add(typeof(Tokens.ForceToken));
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.Attacker.State.Force--;
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Host.Tokens.HasToken(typeof(Tokens.StressToken)) && Host.Tokens.HasToken(typeof(Tokens.ForceToken)))
                    return true;

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    if (Combat.CurrentDiceRoll.Focuses > 1)
                        return 50;
                    else
                        return 45;
                }

                return 0;
            }
        }

    }
}