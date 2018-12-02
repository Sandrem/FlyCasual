using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class RearAdmiralChiraneau : VT49Decimator
        {
            public RearAdmiralChiraneau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rear Admiral Chiraneau",
                    5,
                    88,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RearAdmiralCharaneauAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 147
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RearAdmiralCharaneauAbility : Abilities.FirstEdition.RearAdmiralChiraneauAbility
    {
        protected override void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.SecondEdition.RearAdmiralChiraneauAction() { Host = HostShip });
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class RearAdmiralChiraneauAction : ActionsList.FirstEdition.RearAdmiralChiraneauAction
    {
        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack)
                return false;

            GenericReinforceToken rtoken = Combat.Attacker.Tokens.GetToken<GenericReinforceToken>();
            if (rtoken == null)
                return false;

            if (!Board.IsShipInFacing(Combat.Attacker, Combat.Defender, rtoken.Facing))
                return false;

            if (Combat.DiceRollAttack.Focuses == 0)
                return false;

            return true;
        }
    }
}