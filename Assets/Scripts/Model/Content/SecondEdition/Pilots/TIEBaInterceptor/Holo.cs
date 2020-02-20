using System;
using System.Linq;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class Holo : TIEBaInterceptor
        {
            public Holo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Holo\"",
                    4,
                    54,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HoloAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ee53482be8e59ff44f272e76c4e8123d.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // At the start of the Engagement Phase, you must transfer 1 of your tokens to another friendly ship at range 0-2.

    public class HoloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            // Blue target locks don't count - they are only as FE legacy
            int tokensCount = HostShip.Tokens.GetAllTokens().Where(n => n.GetType() != typeof(BlueTargetLockToken)).Count();
            if (tokensCount > 0)
            {
                int filteredShipsCount = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count(n => n.ShipId != HostShip.ShipId);
                
                if (filteredShipsCount > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectShipToTransferToken);
                }
            }
        }

        private void AskToSelectShipToTransferToken(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AskWhatTokenToTransfer,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Select a friendly ship to transfer your token",
                imageSource: HostShip
            );
        }

        private void AskWhatTokenToTransfer()
        {
            Messages.ShowInfo("Done!");
            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 2) && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return -ship.PilotInfo.Cost;
        }
    }
}