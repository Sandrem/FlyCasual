using Abilities.SecondEdition;
using BoardTools;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ValenRudor : TIELnFighter
        {
            public ValenRudor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Valen Rudor",
                    3,
                    28,
                    isLimited: true,
                    abilityType: typeof(ValenRudorAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 87
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ValenRudorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterValenRudorAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterValenRudorAbility;
        }

        private void RegisterValenRudorAbility(GenericShip ship)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Defender, HostShip, Combat.ChosenWeapon);
            if (shotInformation.Range <= 1 && ship == Combat.Defender && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), delegate
            {
                Selection.ThisShip = previousSelectedShip;
                Triggers.FinishTrigger();
            });
        }
    }
}
