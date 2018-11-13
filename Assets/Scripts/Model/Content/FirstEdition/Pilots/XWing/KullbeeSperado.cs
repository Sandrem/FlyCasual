using Upgrade;
using ActionsList;
using SubPhases;
using Abilities.FirstEdition;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class KullbeeSperado : XWing
        {
            public KullbeeSperado() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kullbee Sperado",
                    7,
                    26,
                    limited: 1,
                    abilityType: typeof(KullbeeSperadoAbility)
                );

                ModelInfo.SkinName = "Partisan";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KullbeeSperadoAbility : GenericAbility
    {
        private GenericUpgrade SFoilsUpgrade;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckKullbeeSperadoAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckKullbeeSperadoAbility;
        }

        private void CheckKullbeeSperadoAbility(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                SFoilsUpgrade = GetSFoils();
                if (SFoilsUpgrade == null) return;

                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseKullbeeSperadoAbility);
            }
        }

        private void AskToUseKullbeeSperadoAbility(object sender, System.EventArgs e)
        {
            SFoilsUpgrade = GetSFoils();
            if (SFoilsUpgrade != null)
            {
                AskToUseAbility(NeverUseByDefault, UseKullbeeSperadoAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseKullbeeSperadoAbility(object sender, System.EventArgs e)
        {
            (SFoilsUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        private GenericUpgrade GetSFoils()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Find(n => n.Types.Contains(UpgradeType.Configuration));
        }
    }
}