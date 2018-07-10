using Ship;
using ActionsList;

namespace Ship
{
    namespace M3AScyk
    {
        public class Serissu : M3AScyk
        {
            public Serissu() : base()
            {
                PilotName = "Serissu";
                PilotSkill = 8;
                Cost = 20;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SerissuAbility());
            }
        }
   }
}
 
 
namespace Abilities
{
    // When another friendly ship at Range 1 is defending, it may reroll 1 defense die.
    public class SerissuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddSerissuAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSerissuAbility;
        }

        private void AddSerissuAbility()
        {
            Combat.Defender.AddAvailableDiceModification(new SerissuAction() { Host = this.HostShip });
        }

        private class SerissuAction : FriendlyRerollAction
        {
            public SerissuAction() : base(1, 1, false, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Serissu's ability";
            }
        }
    }
}
