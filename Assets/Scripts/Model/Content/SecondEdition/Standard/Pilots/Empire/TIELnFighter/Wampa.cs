using System.Collections.Generic;
using Ship;
using Abilities.SecondEdition;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class Wampa : TIELnFighter
        {
            public Wampa() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Wampa\"",
                    "Black Eleven",
                    Faction.Imperial,
                    1,
                    2,
                    2,
                    isLimited: true,
                    abilityType: typeof(WampaAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 89
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WampaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender += RegisterWampaDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterWampaAbility;
            HostShip.OnAttackFinishAsDefender -= RegisterWampaDefenseAbility;
        }

        // Offensive portion
        private void RegisterWampaAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                // give user the option to use ability
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want ot spend 1 Charge to roll 1 additional attack die?",
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
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += WampaAddAttackDice;
            HostShip.RemoveCharge(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void WampaAddAttackDice(ref int value)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= WampaAddAttackDice;
        }

        // defensive portion
        private void RegisterWampaDefenseAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, delegate
                {
                    Messages.ShowInfo("Wampa lost a charge after defending!");
                    HostShip.RemoveCharge(Triggers.FinishTrigger);
                });
            }
        }
    }
}