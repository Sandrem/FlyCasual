using BoardTools;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class JanOrs : Hwk290LightFreighter
        {
            public JanOrs() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jan Ors",
                    5,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JanOrsAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 42
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JanOrsAbility : Abilities.FirstEdition.JanOrsAbility
    {
        protected override void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId
                && Combat.ChosenWeapon.GetType() == typeof(Ship.PrimaryWeaponClass))
            {
                DistanceInfo distanceInfo = new DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4 && Board.IsShipInArc(HostShip, Combat.Attacker))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }
    }
}
