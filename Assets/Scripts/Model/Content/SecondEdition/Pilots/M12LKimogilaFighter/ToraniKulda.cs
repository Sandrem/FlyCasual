using Ship;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class ToraniKulda : M12LKimogilaFighter
        {
            public ToraniKulda() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Torani Kulda",
                    4,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ToraniKuldaAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 207
                );

                ModelInfo.SkinName = "Cartel Executioner";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ToraniKuldaAbility : Abilities.FirstEdition.ToraniKuldaAbility
    {
        protected override void ShowChooseEffect(object sender, System.EventArgs e)
        {
            Selection.ThisShip = (GenericShip)sender;

            ToraniKuldaAbilityDecisionSubPhaseSE subphase = Phases.StartTemporarySubPhaseNew<ToraniKuldaAbilityDecisionSubPhaseSE>(
                "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability",
                delegate {
                    Selection.ThisShip = HostShip;
                    Triggers.FinishTrigger();
                }
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = Selection.ThisShip.ShipId + ": " + "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Suffer 1 damage.", SufferDamage);
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            DamageSourceEventArgs toranikuldaDamage = new DamageSourceEventArgs()
            {
                Source = "Torani Kulda",
                DamageType = DamageTypes.CardAbility
            };

            Selection.ThisShip.Damage.TryResolveDamage(1, toranikuldaDamage, DecisionSubPhase.ConfirmDecision);
        }
    }
}