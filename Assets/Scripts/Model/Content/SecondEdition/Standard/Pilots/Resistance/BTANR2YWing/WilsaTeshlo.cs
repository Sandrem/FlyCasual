using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class WilsaTeshlo : BTANR2YWing
        {
            public WilsaTeshlo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wilsa Teshlo",
                    "Veiled Sorority Privateer",
                    Faction.Resistance,
                    4,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WilsaTeshloAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Orange"
                );

                ImageUrl = "https://i.imgur.com/m8nrMvg.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WilsaTeshloAbility : GenericAbility
    {
        GenericShip defender;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.DamageInfo.IsDefenderDealtFaceUpDamageCard)
            {
                defender = Combat.Defender;
                RegisterAbilityTrigger(TriggerTypes.OnFaceupCritCardIsDealt, AskToMakeChoice);
            }
        }

        private bool AnyHasUsableCharges()
        {
            foreach (GenericUpgrade upgrade in defender.UpgradeBar.GetUpgradesAll())
            {
                if (upgrade.State.Charges > 0 && upgrade.UpgradeInfo.RegensChargesCount == 0) return true;
            }
            return false;
        }

        private void AskToMakeChoice(object sender, EventArgs e)
        {
            LoseChargeSubphase subphase = Phases.StartTemporarySubPhaseNew<LoseChargeSubphase>(
                "Choose one non-recurring charge to lose",
                Triggers.FinishTrigger
            );
            subphase.DescriptionShort = "Choose one non-recurring charge to loose or gain one strain";
            subphase.RequiredPlayer = defender.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;
            subphase.AddDecision(
                    "Gain one Strain",
                    delegate { AssignStrain(); }
                );
            foreach (GenericUpgrade upgrade in GetUsableUpgrades())
            {
                subphase.AddDecision(
                    upgrade.UpgradeInfo.Name,
                    delegate { LoseChargeAndFinish(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.State.Charges
                );
            }
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.Start();
        }

        private void LoseChargeAndFinish(GenericUpgrade upgrade)
        {
            Messages.ShowInfo(defender.PilotInfo.PilotName + " lost 1 charge from " + upgrade.UpgradeInfo.Name);
            upgrade.State.SpendCharge();
            DecisionSubPhase.ConfirmDecision();
        }

        private void AssignStrain()
        {
            Messages.ShowInfo(defender.PilotInfo.PilotName + " gains 1 strain from " + HostShip.PilotInfo.PilotName);
            defender.Tokens.AssignToken(typeof(Tokens.StrainToken), DecisionSubPhase.ConfirmDecision);
        }

        private List<GenericUpgrade> GetUsableUpgrades()
        {
            return defender.UpgradeBar.GetUpgradesAll()
                .Where(n => (n.State.Charges > 0 && n.UpgradeInfo.RegensChargesCount == 0))
                .ToList();
        }

        public class LoseChargeSubphase : DecisionSubPhase { }
    }
}