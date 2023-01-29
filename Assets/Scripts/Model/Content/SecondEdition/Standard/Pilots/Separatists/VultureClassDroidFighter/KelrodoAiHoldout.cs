using Actions;
using ActionsList;
using BoardTools;
using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class KelrodoAiHoldout : VultureClassDroidFighter
    {
        public KelrodoAiHoldout()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Kelrodo-Ai Holdout",
                "Separatist Stalwart",
                Faction.Separatists,
                2,
                2,
                7,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.KelrodoAiHoldoutAbility),
                affectedByStandardized: false,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                }
            );

            ShipInfo.ActionIcons.RemoveLinkedAction(typeof(BarrelRollAction), typeof(CalculateAction));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));

            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Complex);
            DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Complex);

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/4ea8c128-aa07-420e-8046-9adffd1180e6/SWZ97_KelrodoAIHoldoutlegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you are destroyed, you may transfer each of your locks and green tokens to another Kelrodo-Al Holdout at range 0-3. 
    public class KelrodoAiHoldoutAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += TryRegisterDestructionAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= TryRegisterDestructionAbility;
        }

        private void TryRegisterDestructionAbility(GenericShip ship, bool isFled)
        {
            List<GenericToken> TransferrableTokens = HostShip.Tokens.GetTokensByColor(Tokens.TokenColors.Green, Tokens.TokenColors.Blue);
            if (TransferrableTokens.Count > 0
                && Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 3), Team.Type.Friendly).FindAll(s => s.PilotInfo.PilotName == "Kelrodo-Ai Holdout" && s.ShipId != HostShip.ShipId).Count > 0)
            {
                foreach (GenericToken Token in TransferrableTokens)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, (s, e) => AskSelectFriendlyShip(Token));
                }
            }
        }

        private void AskSelectFriendlyShip(GenericToken token)
        {
            String TokenName = (token is BlueTargetLockToken) ? $"Lock \"{(token as BlueTargetLockToken).Letter}\"" : token.Name;

            SelectTargetForAbility(
                () => ShipIsSelected(token),
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Select friendly Kelrodo-Ai Holdout ship at range 0-3 to transfer " + TokenName + " to",
                imageSource: HostShip,
                showSkipButton: true

            );
        }

        private void ShipIsSelected(GenericToken Token)
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            Messages.ShowInfo("Token: " + Token.Name + " transfered from " + HostShip.PilotInfo.PilotName + " to " + TargetShip.PilotInfo.PilotName);
            ActionsHolder.ReassignToken(Token, HostShip, TargetShip, Triggers.FinishTrigger);

        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly)
                && FilterTargetsByRange(ship, 0, 3)
                && ship.PilotInfo.PilotName == "Kelrodo-Ai Holdout"
                && ship.ShipId != HostShip.ShipId;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}
