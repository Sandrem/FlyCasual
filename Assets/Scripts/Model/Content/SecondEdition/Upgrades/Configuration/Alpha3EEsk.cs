using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Alpha3EEsk : GenericUpgrade
    {
        public Alpha3EEsk() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Alpha-3E \"Esk\"",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.NimbusClassVWing.NimbusClassVWing)),
                abilityType: typeof(Abilities.SecondEdition.Alpha3EEskAbility),
                charges: 2,
                regensCharges: true
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/08/3a/083ae697-4a97-4e74-bcb0-77cb590e51e3/swz80_upgrade_alpha-3e.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Alpha3EEskAbility : GenericAbility
    {
        private GenericShip Defender;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckAbility;
            if (Defender != null)
            {
                Defender.OnTryDamagePrevention -= RegisterConvertCritsToIonization;
                Defender.OnAttackFinishAsDefender -= DoCleanup;
            }
        }

        private void CheckAbility()
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostUpgrade.State.Charges >= 2
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, RegisterEskAbility);
            }
        }

        private void RegisterEskAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                SwitchToIonCrits,
                descriptionLong: "Do you want to spend 2 charges to inflict Ion tokens instead of crits?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void SwitchToIonCrits(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharges(2);

            Defender = Combat.Defender;
            Defender.OnTryDamagePrevention += RegisterConvertCritsToIonization;
            Defender.OnAttackFinishAsDefender += DoCleanup;

            Triggers.FinishTrigger();
        }

        private void DoCleanup(GenericShip ship)
        {
            Defender.OnTryDamagePrevention -= RegisterConvertCritsToIonization;
            Defender.OnAttackFinishAsDefender -= DoCleanup;
        }

        private void RegisterConvertCritsToIonization(GenericShip ship, DamageSourceEventArgs e)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, ConvertCritsToIonization);
        }

        private void ConvertCritsToIonization(object sender, EventArgs e)
        {
            int critsCount = Defender.AssignedDamageDiceroll.CriticalSuccesses;
            
            if (critsCount > 0)
            {
                Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {critsCount} crits are converted to Ion Tokens");
                for (int i = 0; i < critsCount; i++)
                {
                    Defender.AssignedDamageDiceroll.RemoveType(DieSide.Crit);
                }
            }

            Defender.Tokens.AssignTokens(
                () => new Tokens.IonToken(Defender),
                critsCount,
                Triggers.FinishTrigger
            );
        }
    }
}