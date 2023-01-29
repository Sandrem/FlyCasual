using Ship;
using Upgrade;
using System;
using BoardTools;
using SubPhases;
using System.Collections.Generic;
using SquadBuilderNS;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Notorious : GenericUpgrade
    {
        public Notorious() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Notorious",
                UpgradeType.Talent,
                cost: 5,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.NotoriousAbility),
                charges: 2,
                regensCharges: true
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/notorious.png";
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            int installedIllicitUpgrades = HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Illicit).Count;
            if (installedIllicitUpgrades == 0)
            {
                Messages.ShowError($"{HostShip.PilotInfo.PilotName} has {UpgradeInfo.Name} upgrade, but doesn't have any Illicit upgrades");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NotoriousAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAbility;
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                new List<DieSide> { DieSide.Blank }
            );
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAbility;
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.Defender.IsStrained);
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }

        private void CheckAbility(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapons);
            if (shotInfo.InArc && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to spend 1 Charge? (If you do, the attacker gains 1 strain token.",
                imageHolder: HostUpgrade
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            Combat.Attacker.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}

