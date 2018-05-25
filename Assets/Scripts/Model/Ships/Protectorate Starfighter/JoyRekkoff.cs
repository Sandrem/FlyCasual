using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using Tokens;
using RuleSets;
using System.Linq;
using Upgrade;

namespace Ship
{
	namespace ProtectorateStarfighter
	{
		public class JoyRekkoff : ProtectorateStarfighter, ISecondEditionPilot
		{
			public JoyRekkoff() : base()
			{
				PilotName = "Joy Rekkoff";
				PilotSkill = 4;
				Cost = 57;

                ImageUrl = "https://i.imgur.com/O4xI9p6.png";

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.JoyRekkoffAbility());
			}

            public void AdaptPilotToSecondEdition()
            {
                // No adaptation is required
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
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskYoUseJoyRekkoffAbility);
                }
            }

            private void AskYoUseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                if (HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0))
                {
                    AskToUseAbility(AlwaysUseByDefault, UseJoyRekkoffAbility);
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private void UseJoyRekkoffAbility(object sender, System.EventArgs e)
            {
                GenericSecondaryWeapon torpedo = (GenericSecondaryWeapon) HostShip.UpgradeBar.GetUpgradesOnlyFaceup().FirstOrDefault(n => n.Types.Contains(UpgradeType.Torpedo) && n.Charges > 0);
                torpedo.SpendCharge(AssignConditionToDefender);
            }

            private void AssignConditionToDefender()
            {
                if (Combat.Defender.Agility != 0)
                {
                    Messages.ShowError("Joy Rekkoff: Agility is decreased");
                    Combat.Defender.Tokens.AssignCondition(new Conditions.JoyRekkoffCondition(Combat.Defender));
                    Combat.Defender.ChangeAgilityBy(-1);
                    Combat.Defender.OnAttackFinish += RemoveJoyRekkoffAbility;
                }
                DecisionSubPhase.ConfirmDecision();
            }

            private void RemoveJoyRekkoffAbility(GenericShip ship)
            {
                Messages.ShowInfo("Agility is restored");
                Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.JoyRekkoffCondition));
                ship.ChangeAgilityBy(+1);
                ship.OnAttackFinish -= RemoveJoyRekkoffAbility;
            }
        }
    }
}

namespace Conditions
{
    public class JoyRekkoffCondition : GenericToken
    {
        public JoyRekkoffCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
            Tooltip = new Ship.ProtectorateStarfighter.JoyRekkoff().ImageUrl;
        }
    }
}