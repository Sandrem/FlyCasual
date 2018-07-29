using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using RuleSets;

namespace Ship
{
    namespace BWing
    {
        public class BraylenStramm : BWing, ISecondEditionPilot
        {
            public BraylenStramm() : base()
            {
                RequiredMods.Add(typeof(MyOtherRideIsMod));

                PilotName = "Braylen Stramm";
                PilotSkill = 3;
                Cost = 24;

                ImageUrl = "https://i.imgur.com/V6m7JN9.png";

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.BraylenStrammPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                RequiredMods.Clear();
                PilotSkill = 4;
                Cost = 50;
                ImageUrl = null;

                PilotAbilities.RemoveAll(ability => ability is Abilities.BraylenStrammPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.BraylenStrammAbility());
            }
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        //While you defend or perform an attack, if you are stressed, you may reroll up to 2 of your dice.
        public class BraylenStrammAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                AddDiceModification(
                    HostName,
                    IsDiceModificationAvailable,
                    GetDiceModificationAiPriority,
                    DiceModificationType.Reroll,
                    2
                );
            }

            public override void DeactivateAbility()
            {
                RemoveDiceModification();
            }

            private bool IsDiceModificationAvailable()
            {
                return HostShip.IsStressed && (HostShip.IsAttacking || HostShip.IsDefending);
            }

            private int GetDiceModificationAiPriority()
            {
                if (HostShip.IsStressed)
                {
                    return 90;
                }
                return 0;
            }
        }
    }
}