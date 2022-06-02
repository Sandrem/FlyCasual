using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Dace Bonearm",
                    "Outer Rim Mercenary",
                    Faction.Scum,
                    4,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DaceBonearmAbility),
                    charges: 3,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
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

        private void CheckAbility(GenericShip ship, GenericToken token)
        {
            if (HostShip.State.Charges < 3) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (token.GetType() != typeof(IonToken)) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range < 4)
            {
                IonizedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskDaceBonearmAbility);
            }
        }

        private void AskDaceBonearmAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldUseAbility,
                UseDaceBonearmAbility,
                descriptionLong: "Do you want to spend 3 Charge? (If you do, that ship gains 2 additional Ion Tokens)",
                imageHolder: HostShip
            );
        }

        private bool ShouldUseAbility()
        {
            return IonizedShip.ShipInfo.BaseSize != BaseSize.Small;
        }

        private void UseDaceBonearmAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + "'s ability has been activated");

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