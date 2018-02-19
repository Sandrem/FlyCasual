namespace Ship
{
    namespace YT2400
    {
        public class EadenVrill : YT2400
        {
            public EadenVrill()
            {
                PilotName = "Eaden Vrill";
                PilotSkill = 3;
                Cost = 32;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.EadenVrillAbility());
            }
        }
    }
}

namespace Abilities
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
