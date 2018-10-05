using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;
using RuleSets;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class SolSixxa : ScurrgH6Bomber, ISecondEditionPilot
        {
            public SolSixxa() : base()
            {
                PilotName = "Sol Sixxa";
                PilotSkill = 6;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SolSixxaAbiliity());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 49;

                PilotAbilities.RemoveAll(a => a is Abilities.SolSixxaAbiliity);
                PilotAbilities.Add(new Abilities.SecondEdition.SolSixxaAbilitySE());

                SEImageNumber = 205;
            }
        }
    }
}

namespace Abilities
{
    public class SolSixxaAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += SolSixxaTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= SolSixxaTemplate;
        }

        protected virtual void SolSixxaTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Turn_1_Left)) availableTemplates.Add(BombDropTemplates.Turn_1_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Turn_1_Right)) availableTemplates.Add(BombDropTemplates.Turn_1_Right);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SolSixxaAbilitySE : SolSixxaAbiliity
    {
        protected override void SolSixxaTemplate(List<BombDropTemplates> availableTemplates)
        {
            base.SolSixxaTemplate(availableTemplates);

            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Left)) availableTemplates.Add(BombDropTemplates.Bank_1_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Right)) availableTemplates.Add(BombDropTemplates.Bank_1_Right);
        }
    }
}
