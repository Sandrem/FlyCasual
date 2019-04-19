using Ship;
using Upgrade;
using Arcs;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class PettyOfficerThanisson : GenericUpgrade
    {
        public PettyOfficerThanisson() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Petty Officer Thanisson",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.PettyOfficerThanissonCrewAbility),
                seImageNumber: 999 //TODO: update this with correct image
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PettyOfficerThanissonCrewAbility : GenericAbility
    {
        private GenericShip ShipWithToken;
        private System.Type token;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += PettyOfficerThanissonEffect;
        }
                
        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= PettyOfficerThanissonEffect;
        }

        private void PettyOfficerThanissonEffect(GenericShip ship, System.Type tokenType)
        {
            TokenColors tokenColor = TokensManager.GetTokenColorByType(tokenType);

            //During the Activation or Engagement Phase, after an enemy ship in your "arc standard
            //front" at range 0-1 gains a red or orange token, if you are not stressed, you may gain 1 stress 
            //token. If you do, that ship gains 1 additional token of the type that it gained.          
            if ((Phases.CurrentPhase is MainPhases.ActivationPhase || Phases.CurrentPhase is MainPhases.CombatPhase) 
                && (tokenColor == TokenColors.Red || tokenColor == TokenColors.Orange)
                && tokenType != typeof(RedTargetLockToken)
                && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && !HostShip.Tokens.HasToken(typeof(StressToken))
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 1)
            {
                ShipWithToken = ship;
                token = tokenType;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                ShouldUseAbility,
                UseAbility,
                infoText: HostShip.PilotInfo.PilotName + ": Gain a Stress Token to assign an additional " + token.Name + " to " + ShipWithToken.PilotInfo.PilotName + "?" 
            );
        }

        private bool ShouldUseAbility()
        {
            return ShipWithToken.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            //TODO: Still having one bug that I identified. Noticed that during Lieutenant Karsabi's ability that replaces a disarm with a stress, 
            //the ability triggered for each and allowed me to gain 2 and assign 2 stress tokens.
            if(token == typeof(TractorBeamToken)){
                HostShip.Tokens.AssignToken(
                    typeof(StressToken), delegate { ShipWithToken.Tokens.AssignToken(new Tokens.TractorBeamToken(ShipWithToken, HostShip.Owner), Triggers.FinishTrigger); }
                );
            } 
            else
            {
                HostShip.Tokens.AssignToken(
                    typeof(StressToken), delegate { ShipWithToken.Tokens.AssignToken(token, Triggers.FinishTrigger); }
                );
            }
        }
    }
}