using Abilities.SecondEdition;
using System;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor
        {
            public SoontirFel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Soontir Fel",
                    6,
                    52,
                    limited: 1,
                    abilityType: typeof(SoontirFelAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Red Stripes";

                SEImageNumber = 103;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SoontirFelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterSoontirFelAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterSoontirFelAbility;
        }

        private void RegisterSoontirFelAbility()
        {
            if (BoardTools.Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, OnCombatAssignFocus);
            }
        }

        private void OnCombatAssignFocus(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }
}