using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class SharaBey : ARC170Starfighter
        {
            public SharaBey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Shara Bey",
                    4,
                    53,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SharaBeyAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 67
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform a primary attack, you may spend 1 lock you have on the enemy ship to add 1 focus result to your dice results.
    public class SharaBeyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Focus,
                payAbilityCost: SpendLock
            );
        }

        private void SpendLock(Action<bool> callback)
        {
            var locks = GetTargetLocksOnEnemy();
            if (locks.Any())
            {
                HostShip.Tokens.SpendToken(
                    typeof(Tokens.BlueTargetLockToken),
                    () => callback(true),
                    locks.First());
            }
            else
            {
                callback(false);
            }

        }

        private int GetDiceModificationAiPriority()
        {
            if (HostShip.Tokens.HasToken<Tokens.FocusToken>())
            {
                return 90;
            }
            else
            {
                return 0;
            }
        }

        private List<char> GetTargetLocksOnEnemy()
        {
            if (Combat.Defender == HostShip)
                return HostShip.Tokens.GetTargetLockLetterPairsOn(Combat.Attacker);
            else if (Combat.Attacker == HostShip)
                return HostShip.Tokens.GetTargetLockLetterPairsOn(Combat.Defender);
            else return new List<char>();
        }

        private bool IsDiceModificationAvailable()
        {
            return (Combat.Defender == HostShip && GetTargetLocksOnEnemy().Any())
                || (Combat.Attacker == HostShip && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon && GetTargetLocksOnEnemy().Any());
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}