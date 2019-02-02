using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESilencer
    {
        public class TestPilotBlackout : TIESilencer
        {
            public TestPilotBlackout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Test Pilot \"Blackout\"",
                    7,
                    31,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TestPilotBlackoutAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TestPilotBlackoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += TryAddTestPilotBlackoutAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= TryAddTestPilotBlackoutAbility;
        }

        private void TryAddTestPilotBlackoutAbility()
        {
            if (Combat.ShotInfo.IsObstructedByAsteroid)
            {
                Combat.Defender.AfterGotNumberOfDefenceDice += DecreaseDiceResult;
            }
        }

        private void DecreaseDiceResult(ref int count)
        {
            Messages.ShowInfo("Test Pilot \"Blackout\":\nDefender rolls 2 fewer dice");
            count -= 2;
            Combat.Defender.AfterGotNumberOfDefenceDice -= DecreaseDiceResult;
        }
    }
}