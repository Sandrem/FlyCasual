using Arcs;
using BoardTools;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class DaceBonearm : HWK290, ISecondEditionPilot
        {
            public DaceBonearm() : base()
            {
                PilotName = "Dace Bonearm";
                PilotSkill = 4;
                Cost = 36;

                UsesCharges = true;
                MaxCharges = 3;
                RegensCharges = true;

                faction = Faction.Scum;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.DaceBonearmTorkilMuxAbility());

                SEImageNumber = 174;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DaceBonearmTorkilMuxAbility : GenericAbility
    {
        private GenericShip IonizedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            if (HostShip.Charges < 3) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (tokenType != typeof(IonToken)) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range < 4)
            {
                IonizedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskDaceBonearmAbility);
            }
        }

        private void AskDaceBonearmAbility(object sender, EventArgs e)
        {
            AskToUseAbility(ShouldUseAbility, UseDaceBonearmAbility);
        }

        private bool ShouldUseAbility()
        {
            return IonizedShip.ShipBaseSize != BaseSize.Small;
        }

        private void UseDaceBonearmAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo("Ability of Dace Bonearm is used");

            for (int i = 0; i < 3; i++)
            {
                //Empty delegate is safe here - Sandrem
                HostShip.RemoveCharge(delegate { });
            }

            IonizedShip.Tokens.AssignToken(
                typeof(IonToken),
                delegate { IonizedShip.Tokens.AssignToken(typeof(IonToken), Triggers.FinishTrigger); }
            );
        }
    }
}
