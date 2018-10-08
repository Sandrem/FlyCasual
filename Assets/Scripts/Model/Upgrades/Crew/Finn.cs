﻿using Upgrade;
using Ship;
using ActionsList;
using UnityEngine;
using Abilities;
using BoardTools;

namespace UpgradesList
{
    public class Finn : GenericUpgrade
    {
        public Finn() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Finn";
            Cost = 5;

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(53, 0));

            isUnique = true;

            UpgradeAbilities.Add(new FinnAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

    }
}

namespace Abilities
{
    public class FinnAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FinnActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FinnActionEffect;
        }

        private void FinnActionEffect(GenericShip host)
        {
            GenericAction newAction = new FinnDiceModification()
            {
                Host = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class FinnDiceModification : GenericAction
    {

        public FinnDiceModification()
        {
            Name = DiceModificationName = "Finn's ability";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Blank).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if ((Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass)) && (Combat.ShotInfo.InArc)) result = true;
                    break;
                case CombatStep.Defence:
                    ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                    if (shotInfo.InArc) result = true;
                    break;
                default:
                    break;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }

    }
}