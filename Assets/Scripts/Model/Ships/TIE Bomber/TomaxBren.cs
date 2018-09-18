using ActionsList;
using RuleSets;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace TIEBomber
    {
        public class TomaxBren : TIEBomber, ISecondEditionPilot
        {
            public TomaxBren() : base()
            {
                PilotName = "Tomax Bren";
                PilotSkill = 8;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.TomaxBrenAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 34;

                PilotAbilities.RemoveAll(a => a is Abilities.TomaxBrenAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.TomaxBrenAbilitySE());

                SEImageNumber = 107;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TomaxBrenAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is ReloadAction && HasEliteUpgradesToRecharge())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ShowEliteUpgradeToRecharge);
            }
        }

        private bool HasEliteUpgradesToRecharge()
        {
            return GetEliteUpgradesToRecharge().Count > 0;
        }

        private List<GenericUpgrade> GetEliteUpgradesToRecharge()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n.Types.Contains(UpgradeType.Elite) && (n.Charges < n.MaxCharges)).ToList();
        }

        private void ShowEliteUpgradeToRecharge(object sender, System.EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<TomaxBrenDecisionSubphase>(
                "Tomax Bren's ability",
                Triggers.FinishTrigger
            );
            phase.InfoText = "You may recover a charge:";
            phase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            phase.ShowSkipButton = true;

            List<GenericUpgrade> AvailableUpgrades = GetEliteUpgradesToRecharge();

            foreach (var upgrade in AvailableUpgrades)
            {
                phase.AddDecision(
                    upgrade.Name,
                    delegate { RestoreCharge(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.Charges
                );
            }

            phase.DefaultDecisionName = AvailableUpgrades[0].Name;

            phase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            phase.Start();
        }

        protected void RestoreCharge(GenericUpgrade upgrade)
        {
            upgrade.RestoreCharge();
            DecisionSubPhase.ConfirmDecision();
        }

        public class TomaxBrenDecisionSubphase : DecisionSubPhase {}
    }
}

namespace Abilities
{
    public class TomaxBrenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade += CheckTomaxBrenAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAfterDiscardUpgrade -= CheckTomaxBrenAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckTomaxBrenAbility(GenericUpgrade upgrade)
        {            
            if (!IsAbilityUsed && upgrade.HasType(UpgradeType.Elite))
            {
                IsAbilityUsed = true;
                Messages.ShowInfo(string.Format("{0} flips {1} face up.", HostShip.PilotName, upgrade.Name));
                RegisterAbilityTrigger(TriggerTypes.OnAfterDiscard, (s, e) => upgrade.TryFlipFaceUp(Triggers.FinishTrigger));
            }
        }                 
    }
}
