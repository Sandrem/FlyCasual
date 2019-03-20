using Upgrade;
using Ship;
using SubPhases;
using ActionsList;
using Tokens;
using System.Linq;
using System;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class SynchronizedConsole : GenericUpgrade
    {
        public SynchronizedConsole() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Synchronized Console",
                UpgradeType.Modification,
                cost: 2,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Republic), 
                    new ActionBarRestriction(typeof(TargetLockAction))),
                abilityType: typeof(Abilities.SecondEdition.SynchronizedConsoleAbility)
                //seImageNumber: ??
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/dd/5a/dd5adbe7-cb13-4d42-8a81-211cd265c210/swz32_synchronized-console.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack, you may choose a friendly ship at range 1 or a friendly ship with the Synchronized Console upgrade 
    //at range 1-3 and spend a lock you have on the defender. If you do, the friendly ship you chose may acquire a lock on the defender.
    public class SynchronizedConsoleAbility : GenericAbility 
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (GetLockOnDefender(HostShip) != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, UseAbility);
            }
        }

        private BlueTargetLockToken GetLockOnDefender(GenericShip ship)
        {
            return ship.Tokens.GetTokens<BlueTargetLockToken>('*').FirstOrDefault(tl => tl.OtherTokenOwner == Combat.Defender);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GetTargetLockOnSameTarget,
                FilterAbilityTarget,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostName,
                "Choose ship to acquire a lock on the defender.",
                HostUpgrade
            );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            if (GetLockOnDefender(ship) != null) return 0;

            var result = 0;

            if (!ship.IsAttackPerformed) result += 100;

            if (BoardTools.Board.IsShipInArc(ship, Combat.Defender)) result += 100;

            result += ship.PilotInfo.Cost;

            return result;
        }

        private bool FilterAbilityTarget(GenericShip ship)
        {
            var abilityRange = ship.UpgradeBar.GetInstalledUpgrades(UpgradeType.Modification).Any(u => u is SynchronizedConsole) ? 3 : 1;

            return FilterByTargetType(ship, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 1, abilityRange);
        }

        private void GetTargetLockOnSameTarget()
        {
            var targetLock = GetLockOnDefender(HostShip);
            if (targetLock != null)
            {
                HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), () =>
                {
                    Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired Target Lock on " + Combat.Defender.PilotInfo.PilotName);
                    ActionsHolder.AcquireTargetLock(TargetShip, Combat.Defender, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection);
                }, targetLock.Letter);
            }
            else
            {
                SelectShipSubPhase.FinishSelection();
            }
        }
    }
}