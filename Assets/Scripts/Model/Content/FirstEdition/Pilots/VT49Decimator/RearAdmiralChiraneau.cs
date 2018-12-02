using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.VT49Decimator
    {
        public class RearAdmiralChiraneau : VT49Decimator
        {
            public RearAdmiralChiraneau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rear Admiral Chiraneau",
                    8,
                    46,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.RearAdmiralChiraneauAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
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


