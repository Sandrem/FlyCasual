using Abilities.FirstEdition;
using Ship;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor
        {
            public SoontirFel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Soontir Fel",
                    9,
                    27,
                    limited: 1,
                    abilityType: typeof(SoontirFelAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Red Stripes";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class SoontirFelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterSoontirFelAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterSoontirFelAbility;
        }

        private void RegisterSoontirFelAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskAssignFocus);
            }
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}