using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class WatTambor : Belbullab22Starfighter
    {
        public WatTambor()
        {
            PilotInfo = new PilotCardInfo(
                "Wat Tambor",
                3,
                44,
                true,
                abilityType: typeof(Abilities.SecondEdition.WatTamborAbility),
                pilotTitle: "Techno Union Foreman",
                extraUpgradeIcon: UpgradeType.Talent
            );

            ModelInfo.SkinName = "Wat Tambor";

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5e/3d/5e3d8e36-3989-40f4-9908-6bd6583bb88a/swz29_wat-tambor.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, you may reroll 1 attack die for each calculating friendly ship at range 1 of the defender.
    public class WatTamborAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                getCount: GetNumberOfDiceToModify
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetNumberOfDiceToModify()
        {
            var count = Roster.AllShips.Values
                .Where(ship => 
                    ship.Owner == HostShip.Owner 
                    && ship.Tokens.HasToken<Tokens.CalculateToken>() 
                    && new DistanceInfo(Combat.Defender, ship).Range == 1)
                .Count();
            
            return count;
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && GetNumberOfDiceToModify() > 0);
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
