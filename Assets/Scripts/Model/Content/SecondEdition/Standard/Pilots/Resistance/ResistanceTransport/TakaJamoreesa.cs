using Abilities.SecondEdition;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ResistanceTransport
    {
        public class TakaJamoreesa : ResistanceTransport
        {
            public TakaJamoreesa() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Taka Jamoreesa",
                    "Snograth Enthusiast",
                    Faction.Resistance,
                    2,
                    4,
                    15,
                    isLimited: true,
                    abilityType: typeof(TakaJamoreesaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/716ee284-e908-4d1d-9f52-ce361d8a88ae/SWZ97_TakaJamoreesalegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TakaJamoreesaAbility : GenericAbility
    {
        GenericShip JammedShip;
        public override void ActivateAbility()
        {
            HostShip.OnJamTargetIsSelected += CheckAssignJam;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnJamTargetIsSelected -= CheckAssignJam;
        }

        private void CheckAssignJam(GenericShip ship)
        {
            if (BoardTools.Board.GetShipsAtRange(ship, new UnityEngine.Vector2(0, 1), Team.Type.Any).Count > 1)
            {
                JammedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToJam);
            }
        }
        private void AskToJam(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You must to assign a Jam token to ship at range 0-1 of " + JammedShip.PilotInfo.PilotName,
                imageSource: HostShip,
                showSkipButton: false
            );
        }

        private void ShipIsSelected()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();
            TargetShip.Tokens.AssignToken(
                new Tokens.JamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return BoardTools.Board.GetShipsAtRange(JammedShip, new UnityEngine.Vector2(0, 1), Team.Type.Any).Contains(ship) && ship.ShipId != JammedShip.ShipId;
        }

        private int GetAiPriority(GenericShip ship)
        {
            int teamModifier = (Tools.IsSameTeam(ship, HostShip)) ? 1 : 10;
            int tokensModifier = (teamModifier == 1) ? 0 : ship.Tokens.GetAllTokens().Count(n => n.TokenColor == Tokens.TokenColors.Green) * 10;
            int shipCostPriority = (teamModifier == 1) ? 200 - ship.PilotInfo.Cost : 200 + ship.PilotInfo.Cost;
            return (shipCostPriority + tokensModifier) * teamModifier;
        }
    }
}
