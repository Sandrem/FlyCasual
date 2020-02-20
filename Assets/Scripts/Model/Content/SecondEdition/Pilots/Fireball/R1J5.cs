using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class R1J5 : Fireball
        {
            public R1J5() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "R1-J5",
                    1,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.R1J5PilotAbility)
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/c5cfd1d89a204722ff95e9a4b134e7f1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // Before you expose 1 of your damage cards, you may look at your facedown damage cards,
    // choose 1, and expose that card instead.
    public class R1J5PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSelectDamageCardToExpose += AskToSelect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSelectDamageCardToExpose -= AskToSelect;
        }

        private void AskToSelect(ref bool isOverriden)
        {
            // Only if player can select another card - only if there are 2 or more facedown damage cards
            if (HostShip.Damage.GetFacedownCards().Count > 1)
            {
                isOverriden = true;

                RegisterAbilityTrigger(TriggerTypes.OnSelectDamageCardToExpose, ShowFacedownDamageCardsToSelect);
            }
        }

        private void ShowFacedownDamageCardsToSelect(object sender, EventArgs e)
        {
            Messages.ShowInfo("Here you should select a damage card");

            Triggers.FinishTrigger();
        }
    }
}