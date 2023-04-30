using Bombs;
using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Cat : Mg100StarFortress
        {
            public Cat() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cat",
                    "Cobalt Wasp",
                    Faction.Resistance,
                    1,
                    5,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CatAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Cobalt";
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