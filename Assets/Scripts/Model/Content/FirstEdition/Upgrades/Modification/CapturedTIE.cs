using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class CapturedTIE : GenericUpgrade
    {
        public CapturedTIE() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Captured TIE",
                UpgradeType.Modification,
                cost: 1,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.CapturedTIEAbility),
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(typeof(Ship.FirstEdition.TIEFighter.TIEFighter))
                )
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CapturedTIEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
            HostShip.OnAttackFinishAsAttacker += RegisterDiscardOnAttackTrigger;
            GenericShip.OnDestroyedGlobal += RegisterDiscardOnLastFriendlyDestroyedTrigger;
            Phases.Events.OnPlanningPhaseStart += RegisterDiscardOnGameStartBeingSingleFriendlyTrigger;
            HostShip.OnFlipFaceUpUpgrade += RegisterDiscardOnFlipFaceUpBeingSingleFriendlyTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryPerformAttackGlobal -= CanPerformAttack;
            HostShip.OnAttackFinishAsAttacker -= RegisterDiscardOnAttackTrigger;
            GenericShip.OnDestroyedGlobal -= RegisterDiscardOnLastFriendlyDestroyedTrigger;
            Phases.Events.OnPlanningPhaseStart -= RegisterDiscardOnGameStartBeingSingleFriendlyTrigger;
            HostShip.OnFlipFaceUpUpgrade -= RegisterDiscardOnFlipFaceUpBeingSingleFriendlyTrigger;
        }

        protected void RegisterDiscardOnAttackTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, (s, e) =>
            {
                Messages.ShowInfoToHuman(string.Format("{0} discarding Captured TIE due to attacking", HostShip.PilotInfo.PilotName));
                HostUpgrade.TryDiscard(Triggers.FinishTrigger);
            });
        }

        protected bool CheckAnyOtherFriendlyShipsPresent()
        {
            return HostShip.Owner.Ships.Values.Any(friendly => friendly.ShipId != HostShip.ShipId && !friendly.IsDestroyed);
        }

        protected void RegisterTriggerSingleFriendlyCheckCommon(string text, TriggerTypes eventType)
        {
            var areThereAnyOtherEnemyShipsThanTheCapturedTie = CheckAnyOtherFriendlyShipsPresent();
            if (!areThereAnyOtherEnemyShipsThanTheCapturedTie)
            {
                RegisterAbilityTrigger(eventType, (s, e) =>
                {
                    Messages.ShowInfoToHuman(text);
                    HostUpgrade.TryDiscard(Triggers.FinishTrigger);
                });
            }
        }

        protected void RegisterDiscardOnLastFriendlyDestroyedTrigger(GenericShip ship, bool isFled)
        {
            RegisterTriggerSingleFriendlyCheckCommon(string.Format("{0} discarding Captured TIE after last friendly ship was destroyed", HostShip.PilotInfo.PilotName), TriggerTypes.OnShipIsDestroyed);
        }

        protected void RegisterDiscardOnGameStartBeingSingleFriendlyTrigger()
        {
            RegisterTriggerSingleFriendlyCheckCommon(string.Format("{0} discarding Captured TIE due to being the only ship in team", HostShip.PilotInfo.PilotName), TriggerTypes.OnPlanningSubPhaseStart);
        }

        protected void RegisterDiscardOnFlipFaceUpBeingSingleFriendlyTrigger()
        {
            RegisterTriggerSingleFriendlyCheckCommon(string.Format("{0} immediately discarding Captured TIE again, due to being the only friendly ship left", HostShip.PilotName), TriggerTypes.OnFlipFaceUp);
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            var isNotTheCapturedTie = Selection.AnotherShip.ShipId != HostShip.ShipId;
            var hasSameOrHigherPS = Selection.ThisShip.State.Initiative >= HostShip.State.Initiative;
            var areThereAnyOtherEnemyShipsThanTheCapturedTie = CheckAnyOtherFriendlyShipsPresent();

            var allowedToBeAttacked = isNotTheCapturedTie || hasSameOrHigherPS || !areThereAnyOtherEnemyShipsThanTheCapturedTie;

            if (!allowedToBeAttacked)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    stringList.Add("Captured TIE: You cannot attack target ship");
                }
                result = false;
            }
        }
    }
}