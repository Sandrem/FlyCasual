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
            var distanceInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distanceInfo.Range <= 1 && Combat.Defender.Owner == HostShip.Owner)
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
