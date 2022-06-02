using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using Tokens;
using BoardTools;
using UnityEngine;
using Content;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class AnakinSkywalker : Delta7Aethersprite
    {
        public AnakinSkywalker()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Anakin Skywalker",
                "Hero of the Republic",
                Faction.Republic,
                6,
                6,
                10,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Anakin Skywalker"
            );

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/d60f4eca355471465ca3f6b99fb98e56.png";
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
            if (HostShip.IsStressed == true)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseAnakin);
            }
        }

        private void AskToUseAnakin(object sender, System.EventArgs e)
        {
            if (isAnakinAbilityAvailable(HostShip) && HostShip.IsStressed == true)
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAnakinAbility,
                    descriptionLong: "Do you want to spend 1 Force to remove 1 Stress Token?",
                    imageHolder: HostShip
                );
            }
            else
            {
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
                    if (HostShip.SectorsInfo.RangeToShipBySector(s, Arcs.ArcType.Front) <= 1) {
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
