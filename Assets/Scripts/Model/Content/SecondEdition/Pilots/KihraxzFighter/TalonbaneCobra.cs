using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class TalonbaneCobra : KihraxzFighter
        {
            public TalonbaneCobra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Talonbane Cobra",
                    5,
                    50,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TalonbaneCobraAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 191
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TalonbaneCobraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice += TalonbaneCobraDiceCheck;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice -= TalonbaneCobraDiceCheck;
        }

        private void TalonbaneCobraDiceCheck(ref int diceCount)
        {
            if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is attacking at range 1, gaining +1 attack die");
                diceCount++;
            }
            if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.Range == 3)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is defending at range 3, gaining +1 defense die");
                diceCount++;
            }
        }
    }
}