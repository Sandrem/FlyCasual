using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class CaptainJonusSSP : TIESaBomber
        {
            public CaptainJonusSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Jonus",
                    "Disciplined Instructor",
                    Faction.Imperial,
                    4,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainJonusSSPAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Device
                    },
                    legality: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal },
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(FeedbackPing));
                MustHaveUpgrades.Add(typeof(PlasmaTorpedoes));
                MustHaveUpgrades.Add(typeof(ProtonBombs));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/captainjonus-swz105.png";

                PilotNameCanonical = "captainjonus-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJonusSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddCaptainJonusSSPAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddCaptainJonusSSPAbility;
        }

        private void AddCaptainJonusSSPAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new CaptainJonusSSPAction(), HostShip);
        }

        private class CaptainJonusSSPAction : FriendlyRerollAction
        {
            public CaptainJonusSSPAction() : base(2, 1, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Captain Jonus";
                ImageUrl = new Ship.SecondEdition.TIESaBomber.CaptainJonusSSP().ImageUrl;
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