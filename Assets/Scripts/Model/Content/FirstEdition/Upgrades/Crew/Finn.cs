﻿using Ship;
using Upgrade;
using UnityEngine;
using ActionsList;
using BoardTools;

namespace UpgradesList.FirstEdition
{
    public class Finn : GenericUpgrade
    {
        public Finn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Finn",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.FinnAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(53, 0));
        }        
    }
}

namespace Abilities.FirstEdition
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
            GenericAction newAction = new ActionsList.FirstEdition.FinnDiceModification()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.FirstEdition
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
                    if ((Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon) && CheckArcRequirements(Combat.Attacker, Combat.Defender)) result = true;
                    break;
                case CombatStep.Defence:
                    result = CheckArcRequirements(Combat.Defender, Combat.Attacker);
                    break;
                default:
                    break;
            }

            return result;
        }

        protected virtual bool CheckArcRequirements(GenericShip thisShip, GenericShip anotherShip)
        {
            ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
            return (shotInfo.InArc);
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }

    }
}