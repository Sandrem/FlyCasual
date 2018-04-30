using SubPhases;
using Upgrade;
using System.Linq;
using System.Collections.Generic;

namespace Ship
{
    namespace M3AScyk
    {
        public class QuinnJast : M3AScyk
        {
            public QuinnJast() : base()
            {
                PilotName = "Quinn Jast";
                PilotSkill = 6;
                Cost = 18;

                IsUnique = true;
                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.QuinnJastAbility());
            }
        }
   }
}
 
 
namespace Abilities
{
    // At the start of the Combat phase, you may receive a weapons disabled token to flip one of your discarded Torpedo or Missile Upgrade cards faceup.
    public class QuinnJastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterAbilityTrigger;
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
            return HostShip.UpgradeBar.GetUpgradesOnlyDiscarded().Where(n => n.hasType(UpgradeType.Missile) || n.hasType(UpgradeType.Torpedo)).ToList();
        }
        
        private void UseQuinnJastAbility(object sender, System.EventArgs e)
        {
            //Simply flip the first discarded missile or torpedo, since the M3-A can never equip more than one (per the current rules at least). 
            //Needs to be updated for HotAC if this ability will be used on a ship with multiple missile/torpedo slots
            var discardedUpgrade = GetDiscardedMissilesOrTorpedoes().FirstOrDefault();

            if (discardedUpgrade != null)
            {
                HostShip.Tokens.AssignToken(new Tokens.WeaponsDisabledToken(HostShip), () => {
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
