using Ship;
using Upgrade;
using System;
using SubPhases;
using BoardTools;
using System.Collections.Generic;
using Tokens;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class JangoFett : GenericUpgrade
    {
        public JangoFett() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jango Fett",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Separatists, Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.JangoFettCrewAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(239, 1)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5b/54/5b5418d8-1e33-403d-abfc-815cf4ffac94/swz82_a1_upgrade_jango-fett.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class JangoFettCrewAbility : GenericAbility
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
                timing: DiceModificationTimingType.Opposite,
                payAbilityCost: SpendLockOnEnemyShip
            );
        }

        private bool IsAvailable()
        {
            GenericShip anotherShip = null;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    anotherShip = Combat.Attacker;
                    break;
                case CombatStep.Defence:
                    anotherShip = Combat.Defender;
                    break;
                default:
                    break;
            }

            return anotherShip != null
                && ActionsHolder.HasTargetLockOn(HostShip, anotherShip)
                && Combat.CurrentDiceRoll.HasResult(DieSide.Focus);
        }

        private void SpendLockOnEnemyShip(Action<bool> callback)
        {
            GenericShip anotherShip = null;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    anotherShip = Combat.Attacker;
                    break;
                case CombatStep.Defence:
                    anotherShip = Combat.Defender;
                    break;
                default:
                    break;
            }

            List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(HostShip, anotherShip);
            if (letters.Count > 0)
            {
                HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), delegate { callback(true); }, letters.First());
            }
            else
            {
                Messages.ShowError("No lock to spend");
                callback(false);
            }
        }

        private int GetAiPriority()
        {
            return 0;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}