﻿using Upgrade;
using Ship;
using RuleSets;
using BoardTools;

namespace UpgradesList
{
    public class Fearless : GenericUpgrade
    {
        public Fearless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Fearless";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.FearlessAbility());

            SEImageNumber = 6;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class FearlessAbility : FearlessnessAbility
        {
            protected override void FearlessnessAddDiceModification(GenericShip host)
            {
                ActionsList.GenericAction newAction = new ActionsList.FearlessAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip
                };
                HostShip.AddAvailableDiceModification(newAction);
            }
        }
    }
}

namespace ActionsList
{
    public class FearlessAction : GenericAction
    {
        public FearlessAction()
        {
            Name = DiceModificationName = "Fearless";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (!(Combat.ChosenWeapon is PrimaryWeaponClass)) return false;

            if (!Combat.ShotInfo.InPrimaryArc) return false;

            if (Combat.ShotInfo.Range != 1) return false;

            ShotInfo reverseShotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            if (!reverseShotInfo.InPrimaryArc) return false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            if (Combat.DiceRollAttack.WorstResult == DieSide.Blank || Combat.DiceRollAttack.WorstResult == DieSide.Focus) return 100;
            return 0;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(Combat.DiceRollAttack.WorstResult, DieSide.Success);
            callBack();
        }
    }
}