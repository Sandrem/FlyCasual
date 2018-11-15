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
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.RearAdmiralCharaneauAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 147;
            }
        }
    }
}

namespace ActionsList.FirstEdition
{
    public class RearAdmiralChiraneauAction : GenericAction
    {
        public RearAdmiralChiraneauAction()
        {
            Name = DiceModificationName = "Rear Admiral Chiraneau's ability";
            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) && (Combat.ShotInfo.Range < 3)) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            if (Combat.DiceRollAttack.Focuses > 0)
                return 100;

            return 0;
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

namespace Abilities.FirstEdition
{
    public class RearAdmiralChiraneauAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddRearAdmiralChiraneauPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddRearAdmiralChiraneauPilotAbility;
        }

        protected virtual void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.FirstEdition.RearAdmiralChiraneauAction());
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
