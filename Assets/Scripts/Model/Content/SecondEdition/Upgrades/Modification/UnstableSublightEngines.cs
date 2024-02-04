using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class UnstableSublightEngines : GenericUpgrade
    {
        public UnstableSublightEngines() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Unstable Sublight Engines",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.UnstableSublightEnginesAbility)
            );

            ImageUrl = "https://i.imgur.com/Cbkyau3.jpg";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you are destroyed, each other ship at range 0-1 suffers 1 damage.
    public class UnstableSublightEnginesAbility : GenericAbility
    {
        private GenericShip oldActiveShip;
        private GenericShip oldThisShip;

        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool isFled)
        {
            if (!isFled) RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, RegisterMoveShip);
        }
        
        private void RegisterMoveShip(object sender, System.EventArgs e)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, MoveShip);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, RegisterDealDamage);
        }

        private void MoveShip(object sender, EventArgs e)
        {
            oldActiveShip = Selection.ActiveShip;
            oldThisShip = Selection.ThisShip;
            Selection.ActiveShip = HostShip;
            Selection.ThisShip = HostShip;

            HostShip.SetAssignedManeuver(new Movement.StraightMovement(1, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.MovementComplexity.Normal), isSilent: true);

            GameManagerScript.Instance.StartCoroutine(PerformAssignedManeuverCoroutine());
        }

        private IEnumerator PerformAssignedManeuverCoroutine()
        {
            yield return Selection.ThisShip.AssignedManeuver.Perform();
            Selection.ActiveShip.Owner.AfterShipMovementPrediction();
        }

        private void RegisterDealDamage()
        {
            Selection.ActiveShip = oldActiveShip;
            Selection.ThisShip = oldThisShip;

            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, DealDamage);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void DealDamage(object sender, EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Values)
            {
                if (ship.ShipId == HostShip.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 2) sufferedShips.Add(ship);
            }

            Messages.ShowInfo("Deadman's Switch deals 1 Hit to " + sufferedShips.Count + " ships");
            DealDamageToShips(sufferedShips, 1, false, Triggers.FinishTrigger);
        }
    }
}