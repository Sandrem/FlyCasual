using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class StructuralDamage : GenericCriticalHit
    {
        public StructuralDamage()
        {
            Name = "Structural Damage";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9b/2e/9b2ed16f-eaa5-4b3c-a9b5-682d5fb9cb2e/structural-damage-card.png";
            CancelDiceResults.Add(DiceSide.Success);
        }

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Agility is reduced by 1");
            Game.UI.AddTestLogEntry("Agility is reduced by 1");
            host.AssignToken(new Tokens.StructuralDamageCritToken());

            host.AfterGetAgility += ReduceAgility;
            Roster.UpdateShipStats(host);

            host.AfterGenerateAvailableActionsList += AddCancelCritAction;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Agility is restored");
            Game.UI.AddTestLogEntry("Agility is restored");
            host.RemoveToken(typeof(Tokens.StructuralDamageCritToken));

            host.AfterGetAgility -= ReduceAgility;
            Roster.UpdateShipStats(host);

            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void ReduceAgility(ref int value)
        {
            value--;
        }

    }

}