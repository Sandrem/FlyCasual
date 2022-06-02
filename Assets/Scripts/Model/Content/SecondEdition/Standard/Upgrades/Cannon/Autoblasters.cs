using Arcs;
using BoardTools;
using Ship;
using System.Linq;
using Upgrade;
using Content;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class Autoblasters : GenericSpecialWeapon
    {
        public Autoblasters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Autoblasters",
                UpgradeType.Cannon,
                cost: 7,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.Front
                ),
                abilityType: typeof(Abilities.SecondEdition.AutoblastersAbility),
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e5/8d/e58d4426-4b4b-4fed-a2e9-0ed970600df5/swz45_autoblasters.png";            
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class AutoblastersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {            
            HostShip.AfterGotNumberOfAttackDice += CheckForExtraDie;
            HostShip.OnDefenceStartAsAttacker += MakeCritsUncancellable;
        }


        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckForExtraDie;
            HostShip.OnDefenceStartAsAttacker -= MakeCritsUncancellable;
        }

        private void CheckForExtraDie(ref int diceAmount)
        {
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {   
                if (Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye))
                {
                    Messages.ShowInfo("Target is in bullseye arc, Autoblaster rolls +1 attack die");
                    diceAmount++;
                }
            }
        }

        private void MakeCritsUncancellable()
        {
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType() && !Combat.Defender.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Front))
            {
                foreach (Die die in Combat.DiceRollAttack.DiceList)
                {
                    if (die.Side == DieSide.Crit) die.IsUncancelable = true;
                }
            }
        }
    }
}