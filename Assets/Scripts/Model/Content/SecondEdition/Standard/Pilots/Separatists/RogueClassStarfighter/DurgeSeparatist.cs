using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class DurgeSeparatist : RogueClassStarfighter
        {
            public DurgeSeparatist() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Durge",
                    "On His Own Time",
                    Faction.Separatists,
                    5,
                    4,
                    10,
                    isLimited: true,
                    charges: 1,
                    abilityType: typeof(Abilities.SecondEdition.DurgeSeparatistAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                PilotNameCanonical = "durge-separatistalliance";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/b3ba7d17-a6f3-4b6d-9740-f85cef641388/SWZ97_Durgelegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DurgeSeparatistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageWasSuccessfullyDealt += RegisterAbility;
        }


        public override void DeactivateAbility()
        {
            HostShip.OnDamageWasSuccessfullyDealt += RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool flag)
        {
            if (HostShip.State.Charges > 0 && HostShip.Damage.HasFacedownCards && HostShip.AssignedDamageDiceroll.Count == 0 && HostShip.Damage.CountAssignedDamage() >= HostShip.ShipInfo.Hull)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageWasSuccessfullyDealt, CheckAbility);
            }
        }

        private void CheckAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0 && HostShip.Damage.HasFacedownCards)
            {
                AskToUseAbility(HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAbility,
                    dontUseAbility: delegate { HostShip.DestroyShipForced(Triggers.FinishTrigger); },
                    descriptionLong: "Do you want to spend 1 charge to Charge to reveal all of your facedown damage, discard each Direct Hit! and each of your damage cards with the Pilot trait, then repair all of your faceup damage cards?",
                    imageHolder: HostShip,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
        }


        private void UseAbility(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.SpendCharge();

            HostShip.OnDamageWasSuccessfullyDealt -= RegisterAbility;

            List<GenericDamageCard> cardsToDiscard = HostShip.Damage.GetFacedownCards().FindAll(d => d.Type == CriticalCardType.Pilot || d.Name == "Direct Hit");
            //foreach (GenericDamageCard card in HostShip.Damage.GetFacedownCards())
            //{
            //    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " revealed: " + card.Name + " type:" + card.Type);
            //}
            //Messages.ShowInfo(HostShip.PilotInfo.PilotName + " revealed " + cardsToDiscard.Count + " Pilot trait and Direct Hit! cards.");

            cardsToDiscard.AddRange(HostShip.Damage.GetFaceupCrits().FindAll(d => d.Type == CriticalCardType.Pilot || d.Name == "Direct Hit"));

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " discarded " + cardsToDiscard.Count + " cards.");

            int remainingDamage = HostShip.Damage.GetFacedownCards().Count + HostShip.Damage.GetFaceupCrits().Count - cardsToDiscard.Count;

            //Messages.ShowInfo(HostShip.PilotInfo.PilotName + " remaining damage: " + remainingDamage);

            if (cardsToDiscard.Count > 0 && remainingDamage < HostShip.ShipInfo.Hull)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has prevented his own destruction");
                HostShip.IsDestroyed = false;
                foreach (GenericDamageCard card in cardsToDiscard)
                {
                    HostShip.Damage.DamageCards.Remove(card);
                    HostShip.CallAfterAssignedDamageIsChanged();
                }

                foreach (GenericDamageCard card in HostShip.Damage.GetFaceupCrits())
                {
                    HostShip.Damage.FlipFaceupCritFacedown(card);
                }
            }
            Triggers.FinishTrigger();

        }
    }
}