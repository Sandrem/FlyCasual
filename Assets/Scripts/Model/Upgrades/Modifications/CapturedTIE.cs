using Ship;
using Upgrade;
using ActionsList;
using System.Linq;
using UnityEngine;
using Ship.TIEFighter;
using System.Collections.Generic;
using System;

namespace UpgradesList
{
    public class CapturedTIE : GenericUpgrade
    {
        public CapturedTIE() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Captured TIE";
            Cost = 1;
            isUnique = true;

            UpgradeAbilities.Add(new Abilities.CapturedTIEAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEFighter && ship.faction == Faction.Rebel;
        }                
    }
}

namespace Abilities
{
    public class CapturedTIEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
            HostShip.OnAttackFinishAsAttacker += DiscardCapturedTIE;            
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryPerformAttackGlobal -= CanPerformAttack;
            HostShip.OnAttackFinishAsAttacker -= DiscardCapturedTIE;
        }

        protected void DiscardCapturedTIE(GenericShip ship)
        {
            HostUpgrade.TryDiscard(() => Messages.ShowInfoToHuman(string.Format("{0} discards Captured TIE", HostShip.PilotName)));            
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            var isNotTheCapturedTie = Selection.AnotherShip.ShipId != HostShip.ShipId;
            var hasSameOrHigherPS = Selection.ThisShip.PilotSkill >= HostShip.PilotSkill;
            var areThereAnyOtherEnemyShipsThanTheCapturedTie = HostShip.Owner.Ships.Values.Any(foe => foe.ShipId != HostShip.ShipId && !foe.IsDestroyed);

            var allowedToBeAttacked = isNotTheCapturedTie || hasSameOrHigherPS || !areThereAnyOtherEnemyShipsThanTheCapturedTie;

            if (!areThereAnyOtherEnemyShipsThanTheCapturedTie)
            {
                DiscardCapturedTIE(HostShip);
            }

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
