using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Sinker : ARC170Starfighter
        {
            public Sinker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Sinker\"",
                    3,
                    50,
                    isLimited: true,
                    factionOverride: Faction.Republic,
                    abilityType: typeof(Abilities.SecondEdition.SinkerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/4e/2b/4e2bb1a3-4865-421d-898f-5272f1ab3b73/swz33_sinker.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship in your left or right arc performs a primary attack, it may reroll 1 attack die.
    public class SinkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        protected virtual bool IsAvailable()
        {
            return
                Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner == HostShip.Owner
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && (HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Left) 
                    || HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Right));            
        }

        private int AiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                var friendlyShip = Combat.Attacker;
                int focuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int blanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (friendlyShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (blanks > 0) result = 90;
                }
                else
                {
                    if (blanks + focuses > 0) result = 90;
                }
            }

            return result;
        }
    }
}