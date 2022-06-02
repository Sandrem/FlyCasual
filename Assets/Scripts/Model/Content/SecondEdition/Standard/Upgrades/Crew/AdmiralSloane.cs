using Ship;
using Upgrade;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Content;

namespace UpgradesList.SecondEdition
{
    public class AdmiralSloane : GenericUpgrade
    {
        public AdmiralSloane() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Admiral Sloane",
                UpgradeType.Crew,
                cost: 16,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.AdmiralSloaneCrewAbility),
                seImageNumber: 109,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(385, 11)
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    //After another friendly ship at range 0-3 defends, if it is destroyed, the attacker gains 2 stress tokens.
    //While a friendly ship at range 0-3 performs an attack against a stressed ship, it may reroll 1 attack die.
    public class AdmiralSloaneCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += RegisterAbility;

            AddDiceModification(
                HostName,
                RerollIsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= RegisterAbility;
            RemoveDiceModification();
        }

        private void RegisterAbility(GenericShip destroyedShip, bool isFled)
        {
            if (Phases.CurrentPhase is MainPhases.CombatPhase && destroyedShip == Combat.Defender && destroyedShip != HostShip && destroyedShip.Owner == HostShip.Owner)
            {
                if (new BoardTools.DistanceInfo(HostShip, destroyedShip).Range > 3) return;

                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AssignStressToAttacker);
            }
        }

        private void AssignStressToAttacker(object sender, System.EventArgs e)
        {
            Combat.Attacker.Tokens.AssignTokens(
                () => new Tokens.StressToken(Combat.Attacker), 
                2, 
                Triggers.FinishTrigger);
        }

        protected virtual bool RerollIsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (Combat.Attacker.Owner != HostShip.Owner) return false;
            if (!Combat.Defender.IsStressed) return false;

            BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 3) return false;

            return true;
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