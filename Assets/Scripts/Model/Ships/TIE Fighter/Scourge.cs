using Arcs;
using BoardTools;
using RuleSets;
using System.Linq;

namespace Ship
{
    namespace TIEFighter
    {
        public class Scourge : TIEFighter, ISecondEditionPilot
        {
            public Scourge()
            {
                PilotName = "\"Scourge\"";
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ScourgeAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // "Scourge" Skutu
                PilotName = PilotName + " Skutu";
                PilotSkill = 5;
                Cost = 32;

                PilotAbilities.RemoveAll(ability => ability is Abilities.ScourgeAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.ScourgeAbilitySE());

                SEImageNumber = 82;
            }
        }
    }
}

namespace Abilities
{
    public class ScourgeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        protected virtual void CheckConditions()
        {
            if (Combat.Defender.Damage.DamageCards.Any())
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }

        protected virtual void SendExtraDiceMessage()
        {
            Messages.ShowInfo("Defender has a damage card. Roll an additional attack die.");
        }

        protected void RollExtraDice(ref int count)
        {
            count++;
            SendExtraDiceMessage();
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScourgeAbilitySE : ScourgeAbility
    {
        protected override void SendExtraDiceMessage()
        {
            Messages.ShowInfo("Defender is in your bullseye arc. Roll an additional attack die.");
        }

        protected override void CheckConditions()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcTypes.Bullseye))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }
    }
}