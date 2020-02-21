using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class LieutenantLeHuse : TIESfFighter
        {
            public LieutenantLeHuse() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant LeHuse",
                    5,
                    39,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantLeHuseAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/b823438eb2b32a407bf6a757a4ecb7d5.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantLeHuseAbility : GenericAbility
    {
        // While you perform an attack, you may spend another friendly ship's lock on the defender
        // to reroll any number of your results.

        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                CheckIsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                payAbilityCost: PayAbilityCost
            );
        }

        private bool CheckIsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && IsLockedByAnotherFriendly(Combat.Defender);
        }

        private bool IsLockedByAnotherFriendly(GenericShip ship)
        {
            return ship.Tokens.GetAllTokens()
                .Where(n => n is RedTargetLockToken)
                .Where(n => ((n as RedTargetLockToken).OtherTargetLockTokenOwner as GenericShip).Owner.PlayerNo == HostShip.Owner.PlayerNo)
                .Where(n => ((n as RedTargetLockToken).OtherTargetLockTokenOwner as GenericShip).ShipId != HostShip.ShipId)
                .Count() != 0;
        }

        private bool HasLockOnDefenderAndIsFriendly(GenericShip ship)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;
            if (ship.ShipId == HostShip.ShipId) return false;

            return ship.Tokens.GetAllTokens()
                .Where(n => n is BlueTargetLockToken)
                .Where(n => ((n as BlueTargetLockToken).OtherTargetLockTokenOwner as GenericShip).ShipId == Combat.Defender.ShipId)
                .Count() != 0;
        }

        private int GetAiPriority()
        {
            return int.MaxValue;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, SelectShipToSpendTL);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate { callback(true); });
        }

        private void SelectShipToSpendTL(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                SpendTLonDefender,
                HasLockOnDefenderAndIsFriendly,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Spend another friendly ship's lock on the defender",
                imageSource: HostShip,
                showSkipButton: false
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(ship, Combat.Defender, ship.PrimaryWeapons.First());
            return (shotInfo.IsShotAvailable) ? 100 - ship.State.Firepower + shotInfo.Range : 100 - ship.PilotInfo.Cost;
        }

        private void SpendTLonDefender()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            List<char> tlLetters = ActionsHolder.GetTargetLocksLetterPairs(TargetShip, Combat.Defender);
            TargetShip.Tokens.SpendToken(typeof(BlueTargetLockToken), Triggers.FinishTrigger, tlLetters.First());
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
