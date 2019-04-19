using Ship;
using System;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Team;
using Tokens;
using Movement;
using BoardTools;
using UnityEngine;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class AnakinSkywalker : Delta7Aethersprite
    {
        public AnakinSkywalker()
        {
            PilotInfo = new PilotCardInfo(
                "Anakin Skywalker",
                6,
                60,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ModelInfo.SkinName = "Anakin Skywalker";

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/d60f4eca355471465ca3f6b99fb98e56.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you fully execute a maneuver, if there is an enemy ship in your front arc at
    // range 0-1 or in your bulls-eye arc, you may spend 1 force token to remove stress token.
    public class AnakinSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterCheckAnakinAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterCheckAnakinAbility;
        }

        private void RegisterCheckAnakinAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseAnakin);
        }

        private void AskToUseAnakin(object sender, System.EventArgs e)
        {
            if (isAnakinAbilityAvailable(HostShip)) {
                AskToUseAbility(AlwaysUseByDefault, UseAnakinAbility);
            } else {
                Triggers.FinishTrigger();
            }
        }

        private void UseAnakinAbility(object sender, System.EventArgs e) 
        {
            HostShip.Tokens.SpendToken(typeof(ForceToken), () => 
                HostShip.Tokens.RemoveToken(
                    typeof(StressToken),
                    DecisionSubPhase.ConfirmDecision
                ));
        }

        private bool isAnakinAbilityAvailable(GenericShip ship)
        {
            if (ship.State.Force > 0 && ship.Owner == HostShip.Owner)
            {
                int enemies = 0;
                enemies += Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Count;
                foreach(var s in Board.GetShipsAtRange(HostShip, new Vector2(0,1), Team.Type.Enemy)) {
                    if (Board.IsShipInArc(HostShip,s)) {
                        enemies++;
                    }
                }
                
                if (enemies > 0) {
                    return true;
                }
            }
            return false;
        }
        
    }
}
