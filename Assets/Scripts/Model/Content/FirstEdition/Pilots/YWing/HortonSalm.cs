using BoardTools;
using Mods;
using Mods.ModsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class HortonSalm : YWing
        {
            public HortonSalm() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Horton Salm",
                    8,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HortonSalmAbility)
                );

                if ((ModsManager.Mods[typeof(EliteYWingPilotsMod)].IsOn)) ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class HortonSalmAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += HortonSalmPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= HortonSalmPilotAbility;
        }

        public void HortonSalmPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new HortonSalmAction());
        }

        private class HortonSalmAction : ActionsList.GenericAction
        {
            public HortonSalmAction()
            {
                Name = DiceModificationName = "Horton Salm's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = new List<DieSide> { DieSide.Blank },
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if ((Combat.AttackStep == CombatStep.Attack) && (shotInfo.Range > 1))
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.DiceRollAttack.Blanks > 0) result = 95;
                }

                return result;
            }
        }

    }
}
