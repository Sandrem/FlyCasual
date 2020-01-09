using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class Chewbacca : ModifiedYT1300LightFreighter
        {
            public Chewbacca() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Chewbacca",
                    4,
                    71,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChewbaccaRebelPilotAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 71
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChewbaccaRebelPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += RegisterChewbaccaTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= RegisterChewbaccaTrigger;
        }

        private void RegisterChewbaccaTrigger(GenericShip ship)
        {
            if (Combat.CurrentCriticalHitCard.IsFaceup && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskUseChewbaccaAbility);
            }
        }

        private void AskUseChewbaccaAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            AskToUseAbility(
                descriptionShort: HostShip.PilotInfo.PilotName,
                IsShouldUseAbility,
                UseAbility,
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    DecisionSubPhase.ConfirmDecision();
                },
                descriptionLong: "Do you want to spend 1 Charge to be dealt the faceup damage card as facedown instead?",
                imageHolder: HostShip
            );
        }

        private bool IsShouldUseAbility()
        {
            return true;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            HostShip.State.Charges--;
            Combat.CurrentCriticalHitCard.IsFaceup = false;
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
