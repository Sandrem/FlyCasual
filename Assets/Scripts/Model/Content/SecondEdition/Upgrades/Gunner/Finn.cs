using Ship;
using Upgrade;
using Arcs;
using ActionsList;
using UnityEngine;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class Finn : GenericUpgrade
    {
        public Finn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Finn",
                UpgradeType.Gunner,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.FinnAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(349, 1)
            );
        }        
    }
}

namespace Abilities.SecondEdition
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
            GenericAction newAction = new ActionsList.SecondEdition.FinnDiceModification()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class FinnDiceModification : ActionsList.FirstEdition.FinnDiceModification
    {
        protected override bool CheckArcRequirements(GenericShip thisShip, GenericShip anotherShip)
        {
            return thisShip.SectorsInfo.IsShipInSector(anotherShip, ArcType.Front);
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
            Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Blank);
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