using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace VT49Decimator
    {
        public class RearAdmiralChiraneau : VT49Decimator
        {
            public RearAdmiralChiraneau() : base()
            {
                PilotName = "Rear Admiral Chiraneau";
                PilotSkill = 8;
                Cost = 46;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new RearAdmiralChiraneauAbility());
            }
        }
    }
}

namespace Abilities
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

        private void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new RearAdmiralChiraneauAction());
        }

        private class RearAdmiralChiraneauAction : ActionsList.GenericAction
        {
            public RearAdmiralChiraneauAction()
            {
                Name = DiceModificationName = "Rear Admiral Chiraneau's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Combat.ShotInfo.Range < 3)
                {
                    Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit);
                }
                else
                {
                    Messages.ShowErrorToHuman("Wrong range");
                }
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
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.ShotInfo.Range < 3)
                    {
                        if (Combat.DiceRollAttack.Focuses > 0) result = 100;
                    }
                }

                return result;
            }
        }

    }
}
