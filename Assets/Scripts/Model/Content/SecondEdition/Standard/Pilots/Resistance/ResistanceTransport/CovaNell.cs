using Abilities.SecondEdition;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ResistanceTransport
    {
        public class CovaNell : ResistanceTransport
        {
            public CovaNell() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cova Nell",
                    4,
                    38,
                    isLimited: true,
                    abilityText: "While you defend or perform a primary attack, if your revealed maneuver is red, roll 1 additional die.",
                    abilityType: typeof(CovaNellAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/1b/93/1b934d61-0f90-42d0-bf84-0052960b105b/swz45_cova-nell.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CovaNellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (HostShip.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.ColorComplexity == Movement.MovementComplexity.Complex)
            {
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 defense die");
                    count++;
                }
                else if (Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
                {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
                    count++;
                }
            }
        }
    }
}
