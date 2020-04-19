using Bombs;
using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Cat : Mg100StarFortress
        {
            public Cat() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cat",
                    1,
                    54,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CatAbility)
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/b386dc25736682ebc785b15551de903b.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is at range 0-1 of at least 1 friendly device, roll 1 additional die.
    public class CatAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckCatAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckCatAbility;
        }

        private void CheckCatAbility(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                foreach (var bombHolder in BombsManager.GetBombsOnBoard())
                {
                    if (bombHolder.Value.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo
                        && BombsManager.IsShipInRange(Combat.Defender, bombHolder.Key, 1))
                    {
                        Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains +1 attack die");
                        count++;
                        return;
                    }
                }
            }
        }
    }
}