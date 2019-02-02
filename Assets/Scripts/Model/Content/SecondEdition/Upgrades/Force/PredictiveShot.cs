using Arcs;
using SubPhases;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PredictiveShot : GenericUpgrade
    {
        public PredictiveShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Predictive Shot",
                UpgradeType.Force,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.PredictiveShotAbility)//,
                                                                        //seImageNumber: 22
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e0100c63b2753daf68a9d3948824b086.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PredictiveShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.State.Force > 0 && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskUsePredictiveShot);
            }
        }

        protected void AskUsePredictiveShot(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, RegisterPredictiveShot, null, null, false);
        }

        protected void RegisterPredictiveShot(object sender, System.EventArgs e)
        {
            HostShip.State.Force--;
            Combat.Defender.AfterGotNumberOfDefenceDiceCap += SetDefenseDic;
            DecisionSubPhase.ConfirmDecision();
        }

        private void SetDefenseDic(ref int result)
        {
            if (Combat.DiceRollAttack.Successes < result) { result = Combat.DiceRollAttack.Successes; }
            Combat.Defender.AfterGotNumberOfDefenceDiceCap -= SetDefenseDic;
        }
    }
}