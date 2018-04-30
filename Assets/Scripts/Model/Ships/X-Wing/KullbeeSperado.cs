using Upgrade;
using Abilities;
using ActionsList;
using System.Linq;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class KullbeeSperado : XWing
        {
            public KullbeeSperado() : base()
            {
                PilotName = "Kullbee Sperado";
                PilotSkill = 7;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                SkinName = "Partisan";

                PilotAbilities.Add(new KullbeeSperadoAbility());
            }
        }
    }
}

namespace Abilities
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
            HostShip.OnActionIsPerformed += CheckKullbeeSperadoAbility;
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
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Find(n => n is UpgradesList.ServomotorSFoilsAttack || n is UpgradesList.ServomotorSFoilsClosed);
        }
    }
}