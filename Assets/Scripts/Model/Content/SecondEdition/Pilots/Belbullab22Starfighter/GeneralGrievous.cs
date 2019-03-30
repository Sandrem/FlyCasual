using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class GeneralGrievous : Belbullab22Starfighter
    {
        public GeneralGrievous()
        {
            PilotInfo = new PilotCardInfo(
                "General Grievous",
                4,
                47,
                true,
                abilityType: typeof(Abilities.SecondEdition.GeneralGrievousAbility),
                pilotTitle: "Ambitious Cyborg",
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e1/9e/e19e3aaa-4b9f-4a9e-bc8f-46812882ebc7/swz29_grievous.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if you are not in the defender’s firing arc, you may reroll up to 2 attack dice.
    public class GeneralGrievousAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                2
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && !new ShotInfo(Combat.Defender, HostShip, Combat.Defender.PrimaryWeapons.First()).InArc);
        }

        public int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }
    }
}
