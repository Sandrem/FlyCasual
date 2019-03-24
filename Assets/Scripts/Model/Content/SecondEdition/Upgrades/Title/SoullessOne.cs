using Upgrade;
using BoardTools;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SoullessOne : GenericUpgrade
    {
        public SoullessOne() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Soulless One",
                UpgradeType.Title,
                cost: 10,       
                isLimited: true,
                addHull: 2,
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.Belbullab22Starfighter.Belbullab22Starfighter)),
                    new FactionRestriction(Faction.Separatists)
                ),
                abilityType: typeof(Abilities.SecondEdition.SoullessOneAbility)
                //seImageNumber: ??
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f3/18/f318bb11-2334-4b13-aee2-3930ed267cf0/swz29_a2_soulless-one.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, if the attack is outside your firing arc, you may reroll 1 defense die.
    public class SoullessOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Defence
                && Combat.Defender == HostShip
                && !new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapons.First()).InArc);
        }

        public int GetDiceModificationAiPriority()
        {
            return 90;
        }
    }
}