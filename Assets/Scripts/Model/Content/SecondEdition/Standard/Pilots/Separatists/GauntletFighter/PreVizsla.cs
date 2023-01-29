using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class PreVizsla : GauntletFighter
        {
            public PreVizsla() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Pre Vizsla",
                    "Leader of Death Watch",
                    Faction.Separatists,
                    3,
                    7,
                    18,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.PreVizslaAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "CIS Light"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/1f/Previzsla.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PreVizslaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterPreVizslaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterPreVizslaAbility;
        }

        // Offensive portion
        private void RegisterPreVizslaAbility()
        {
            if (Combat.Defender.PilotInfo.Initiative >= HostShip.PilotInfo.Initiative)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 1)
            {
                // give user the option to use ability
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want ot spend 2 Charge to roll 1 additional attack die?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += PreVizslaAddAttackDice;
            HostShip.State.Charges -= 2;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void PreVizslaAddAttackDice(ref int value)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= PreVizslaAddAttackDice;
        }
    }
}