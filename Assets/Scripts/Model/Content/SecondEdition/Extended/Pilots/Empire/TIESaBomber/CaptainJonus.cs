using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class CaptainJonus : TIESaBomber
        {
            public CaptainJonus() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Jonus",
                    "Disciplined Instructor",
                    Faction.Imperial,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainJonusAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 108,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJonusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddCaptainJonusAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddCaptainJonusAbility;
        }

        private void AddCaptainJonusAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new CaptainJonusAction(), HostShip);
        }

        private class CaptainJonusAction : FriendlyRerollAction
        {
            public CaptainJonusAction() : base(2, 1, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Captain Jonus";
                ImageUrl = new Ship.SecondEdition.TIESaBomber.CaptainJonus().ImageUrl;
                IsReroll = true;
            }

            protected override bool CanReRollWithWeaponClass()
            {
                if (Combat.ChosenWeapon is GenericSpecialWeapon)
                {
                    GenericSpecialWeapon upgradeWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
                    return upgradeWeapon.HasType(UpgradeType.Missile) || upgradeWeapon.HasType(UpgradeType.Torpedo);
                }

                return false;
            }
        }
    }
}