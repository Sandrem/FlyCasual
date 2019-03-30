using BoardTools;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class Backstabber : TIEFighter
        {
            public Backstabber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Backstabber\"",
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BackstabberAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BackstabberAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += BackstabberPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= BackstabberPilotAbility;
        }

        private void BackstabberPilotAbility(ref int diceNumber)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.ChosenWeapon);
            if (!shotInformation.InArc)
            {
                Messages.ShowInfo("Backstabber is attacking from outside the defender's firing arc and rolls an additional attack die.");
                diceNumber++;
            }
        }
    }
}