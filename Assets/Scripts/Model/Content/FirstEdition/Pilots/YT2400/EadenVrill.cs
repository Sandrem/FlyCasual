namespace Ship
{
    namespace FirstEdition.YT2400
    {
        public class EadenVrill : YT2400
        {
            public EadenVrill() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Eaden Vrill",
                    3,
                    32,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.EadenVrillAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class EadenVrillAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (Combat.Defender.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Defender is stressed. Roll 1 additional attack die.");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}