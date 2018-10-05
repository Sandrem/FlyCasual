using SubPhases;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace M3AScyk
    {
        public class QuinnJast : M3AScyk, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 35;

                PilotAbilities.RemoveAll(ability => ability is Abilities.QuinnJastAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.QuinnJastAbilitySE());

                SEImageNumber = 186;
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

namespace Abilities.SecondEdition
{
    public class QuinnJastAbilitySE : GenericAbility
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
            if (GetHardpointWithSpentCharges() != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseQuinnJastAbility);
            }
        }

        private void AskUseQuinnJastAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseQuinnJastAbility);
        }

        private GenericUpgrade GetHardpointWithSpentCharges()
        {
            foreach(GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesAll())
            {
                if
                (
                    upgrade.HasType(UpgradeType.Missile) ||
                    upgrade.HasType(UpgradeType.Cannon) ||
                    upgrade.HasType(UpgradeType.Torpedo)
                )
                {
                    if(upgrade.UsesCharges && upgrade.Charges < upgrade.MaxCharges)
                    {
                        return upgrade;
                    }
                }
            }

            return null;
        }

        private void UseQuinnJastAbility(object sender, System.EventArgs e)
        {
            GenericUpgrade spentUpgrade = GetHardpointWithSpentCharges();

            if (spentUpgrade != null)
            {
                HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () => {
                    spentUpgrade.RestoreCharge();
                    DecisionSubPhase.ConfirmDecision();
                });
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}