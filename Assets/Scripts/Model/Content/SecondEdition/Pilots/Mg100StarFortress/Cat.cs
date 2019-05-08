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
                    59,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CatAbility) //,
                    //seImageNumber: 19
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/e6/StarFortress_Cat.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
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
                    List<GenericShip> shipsNear = BombsManager.GetShipsInRange(bombHolder.Key);
                    if (shipsNear.Contains(Combat.Defender) && bombHolder.Value.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
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