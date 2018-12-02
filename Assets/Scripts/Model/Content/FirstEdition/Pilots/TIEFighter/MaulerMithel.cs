using BoardTools;
using Abilities.FirstEdition;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class MaulerMithel : TIEFighter
        {
            public MaulerMithel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Mauler Mithel\"",
                    7,
                    17,
                    isLimited: true,
                    abilityType: typeof(MaulerMithelAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MaulerMithelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += MaulerMithelPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= MaulerMithelPilotAbility;
        }

        private void MaulerMithelPilotAbility(ref int result)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if (shotInformation.Range == 1)
            {
                Messages.ShowInfo("\"Mauler Mithel\": +1 attack die");
                result++;
            }
        }
    }
}
