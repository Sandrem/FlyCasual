using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class QuinnJast : M3AInterceptor
        {
            public QuinnJast() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Quinn Jast",
                    6,
                    18,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.QuinnJastAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    // At the start of the Combat phase, you may receive a weapons disabled token to flip one of your discarded Torpedo or Missile Upgrade cards faceup.
    public class QuinnJastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            if (GetDiscardedMissilesOrTorpedoes().Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseQuinnJastAbility);
            }
        }

        private void AskUseQuinnJastAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseQuinnJastAbility);
        }

        private List<GenericUpgrade> GetDiscardedMissilesOrTorpedoes()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyDiscarded().Where(n => n.HasType(UpgradeType.Missile) || n.HasType(UpgradeType.Torpedo)).ToList();
        }

        private void UseQuinnJastAbility(object sender, System.EventArgs e)
        {
            //Simply flip the first discarded missile or torpedo, since the M3-A can never equip more than one (per the current rules at least). 
            //Needs to be updated for HotAC if this ability will be used on a ship with multiple missile/torpedo slots
            var discardedUpgrade = GetDiscardedMissilesOrTorpedoes().FirstOrDefault();

            if (discardedUpgrade != null)
            {
                HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () => {
                    discardedUpgrade.FlipFaceup(DecisionSubPhase.ConfirmDecision);
                });
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}