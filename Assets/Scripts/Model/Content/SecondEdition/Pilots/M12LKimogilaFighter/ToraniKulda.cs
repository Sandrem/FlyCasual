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
                    50,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.ToraniKuldaAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
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

            Phases.StartTemporarySubPhaseOld(
                "Select effect of Torani Kulda's ability",
                typeof(ToraniKuldaAbilityDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            InfoText = Selection.ThisShip.ShipId + ": " + "Select effect of Torani Kulda's ability";
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