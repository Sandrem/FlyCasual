using Upgrade;

namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class FennRau : ProtectorateStarfighter
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    9,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.FennRauScumAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class FennRauScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo("The attack is at range 1.  Fenn Rau gain +1 die.");
                value++;
            }
        }
    }
}
