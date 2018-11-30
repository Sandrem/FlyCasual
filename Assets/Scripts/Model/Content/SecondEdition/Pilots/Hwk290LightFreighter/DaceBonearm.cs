using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class DaceBonearm : Hwk290LightFreighter
        {
            public DaceBonearm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dace Bonearm",
                    4,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DaceBonearmAbility),
                    charges: 3,
                    regensCharges: true,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 174
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DaceBonearmAbility : GenericAbility
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
            if (HostShip.State.Charges < 3) return;

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
            return IonizedShip.ShipInfo.BaseSize != BaseSize.Small;
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