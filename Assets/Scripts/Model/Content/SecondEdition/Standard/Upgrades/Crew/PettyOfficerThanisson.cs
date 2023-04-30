using Ship;
using Upgrade;
using Arcs;
using SubPhases;
//using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class PettyOfficerThanisson : GenericUpgrade
    {
        public PettyOfficerThanisson() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Petty Officer Thanisson",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.PettyOfficerThanissonCrewAbility)
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(308, 1)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PettyOfficerThanissonCrewAbility : GenericAbility
    {
        private GenericShip ShipWithToken;
        private GenericToken Token;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += PettyOfficerThanissonEffect;
        }
                
        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= PettyOfficerThanissonEffect;
        }

        private void PettyOfficerThanissonEffect(GenericShip ship, GenericToken token)
        {
            TokenColors tokenColor = token.TokenColor;

            //During the Activation or Engagement Phase, after an enemy ship in your "arc standard
            //front" at range 0-1 gains a red or orange token, if you are not stressed, you may gain 1 stress 
            //token. If you do, that ship gains 1 additional token of the type that it gained.          
            if ((Phases.CurrentPhase is MainPhases.ActivationPhase || Phases.CurrentPhase is MainPhases.CombatPhase) 
                && (tokenColor == TokenColors.Red || tokenColor == TokenColors.Orange)
                && token.GetType() != typeof(RedTargetLockToken)
                && !HostShip.Tokens.HasToken(typeof(StressToken))
                && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 1)
            {
                ShipWithToken = ship;
                Token = token;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    ShouldUseAbility,
                    UseAbility,
                    descriptionLong: "Do you want to gain a Stress Token to assign an additional " + Token.Name + " to " + ShipWithToken.PilotInfo.PilotName + "?",
                    imageHolder: HostUpgrade
                );
            } 
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUseAbility()
        {
            return ShipWithToken.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
         
            HostShip.Tokens.AssignToken(
                typeof(StressToken), delegate { AssignExtraToken(Token.GetType()); }
            );
        }

        private void AssignExtraToken(System.Type tokenToAssign)
        {
            if (Token is TractorBeamToken)
            {
                GenericToken tractorToken = new TractorBeamToken(ShipWithToken, HostShip.Owner);
                ShipWithToken.Tokens.AssignToken(tractorToken, Triggers.FinishTrigger);
            } 
            else 
            {
                ShipWithToken.Tokens.AssignToken(tokenToAssign, Triggers.FinishTrigger);
            }
            
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Gained a Stress Token to assign an additional " + tokenToAssign.Name 
                + " to " + ShipWithToken.PilotInfo.PilotName + ".");
        }
    }
}