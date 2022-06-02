using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class JangoFett : FiresprayClassPatrolCraft
        {
            public JangoFett() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jango Fett",
                    "Simple Man",
                    Faction.Separatists,
                    6,
                    8,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JangoFettAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d4/f0/d4f09efe-f07f-45ad-a82f-8fdc29ec8f75/swz82_a1_jango-fett.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JangoFettAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Jango Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if (Combat.Defender.ShipId == HostShip.ShipId)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Attacker);
                    }
                    break;
                case CombatStep.Defence:
                    if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon)
                    {
                        result = IsMyRevealedManeuverIsLess(Combat.Defender);
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool IsMyRevealedManeuverIsLess(GenericShip anotherShip)
        {
            bool result = false;

            if (HostShip.RevealedManeuver != null && anotherShip.RevealedManeuver != null)
            {
                if (HostShip.RevealedManeuver.ColorComplexity < anotherShip.RevealedManeuver.ColorComplexity) result = true;
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}