using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bombs;
using Abilities;
using Ship;
using ActionsList;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class CaptainNymScum : ScurrgH6Bomber
        {
            public CaptainNymScum() : base()
            {
                PilotName = "Captain Nym";
                PilotSkill = 8;
                Cost = 30;

                IsUnique = true;

                SkinName = "Captain Nym (Scum)";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainNymScumAbiliity());
            }
        }
    }
}

namespace Abilities
{
    public class CaptainNymScumAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += CheckObstructionBonus;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= CheckObstructionBonus;
        }

        private void CheckObstructionBonus()
        {
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (Combat.AttackStep != CombatStep.Defence) return;

            if (Combat.ShotInfo.IsObstructedByBombToken)
            {
                Combat.Defender.AddAvailableActionEffect(new CaptainNymObstructionBonus());
            }
        }
    }
}

namespace ActionsList
{

    public class CaptainNymObstructionBonus : GenericAction
    {
        public CaptainNymObstructionBonus()
        {
            Name = EffectName = "Captain Nym: Free Evade";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyEvade();
            callBack();
        }

        public override int GetActionEffectPriority()
        {
            return 110;
        }
    }

}
