using ActionsList;
using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScumYT1300
    {
        public class LandoCalrissian : ScumYT1300, ISecondEditionPilot
        {
            public LandoCalrissian() : base()
            {
                PilotName = "Lando Calrissian";
                PilotSkill = 4;
                Cost = 49;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.LandoCalrissianScumPilotAbilitySE());

                SEImageNumber = 223;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianScumPilotAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2,
                new List<DieSide>() { DieSide.Blank },
                timing: DiceModificationTimingType.AfterRolled,
                payAbilityCost: PayAbilityCost
            );
        }

        private bool IsDiceModificationAvailable()
        {
            return true;
        }

        private int GetAiPriority()
        {
            return 95;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StressToken), () => callback(true));
        }
    }
}
