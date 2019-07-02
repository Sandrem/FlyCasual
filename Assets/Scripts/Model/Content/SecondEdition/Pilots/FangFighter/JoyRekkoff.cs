﻿using Ship;
using SubPhases;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class JoyRekkoff : FangFighter
        {
            public JoyRekkoff() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Joy Rekkoff",
                    4,
                    52,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JoyRekkoffAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 157
                );
            }
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class JoyRekkoffAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnAttackStartAsAttacker += CheckAbility;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnAttackStartAsAttacker -= CheckAbility;
            }

            private void CheckAbility()
            {
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.UpgradeInfo.HasType(UpgradeType.Torpedo) && n.State.Charges > 0))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskYoUseJoyRekkoffAbility);
                }
            }

            private void AskYoUseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.UpgradeInfo.HasType(UpgradeType.Torpedo) && n.State.Charges > 0))
                {
                    AskToUseAbility(
                        HostShip.PilotInfo.PilotName,
                        AlwaysUseByDefault,
                        UseJoyRekkoffAbility,
                        descriptionLong: "Do you wand to spend 1 Charge from an equipped Torpedo Upgrade? (If you do, the defender rolls 1 fewer defense die)",
                        imageHolder: HostShip
                    );
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private void UseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                GenericSpecialWeapon torpedo = (GenericSpecialWeapon)HostShip.UpgradeBar.GetUpgradesOnlyFaceup().FirstOrDefault(n => n.UpgradeInfo.HasType(UpgradeType.Torpedo) && n.State.Charges > 0);
                torpedo.State.SpendCharge();
                AssignConditionToDefender();
            }

            private void AssignConditionToDefender()
            {
                Combat.Defender.Tokens.AssignCondition(typeof(Conditions.JoyRekkoffCondition));
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}

namespace Conditions
{
    public class JoyRekkoffCondition : GenericToken
    {
        bool AgilityWasDecreased = false;

        public JoyRekkoffCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = typeof(Ship.SecondEdition.FangFighter.JoyRekkoff);

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            if (Host.State.Agility != 0)
            {
                AgilityWasDecreased = true;

                Messages.ShowInfo("Joy Rekkoff causes " + Host.PilotInfo.PilotName + "'s Agility to be decreased by 1");
                Host.ChangeAgilityBy(-1);
            }

            Host.OnAttackFinishAsDefender += RemoveJoyRekkoffAbility;
        }

        private void RemoveJoyRekkoffAbility(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            if (AgilityWasDecreased)
            {
                Messages.ShowInfo("Joy Rekkoff: " + Host.PilotInfo.PilotName + "'s Agility has been restored");
                Host.ChangeAgilityBy(+1);
            }

            Host.OnAttackFinishAsDefender -= RemoveJoyRekkoffAbility;
        }
    }
}