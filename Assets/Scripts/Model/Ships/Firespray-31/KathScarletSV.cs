using Arcs;
using RuleSets;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KathScarletSV : Firespray31, ISecondEditionPilot
        {
            public KathScarletSV() : base()
            {
                PilotName = "Kath Scarlet";
                PilotSkill = 7;
                Cost = 38;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;

                SkinName = "Kath Scarlet";

                PilotAbilities.Add(new Abilities.KathScarletSVAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 74;

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Illicit);

                PilotAbilities.RemoveAll(a => a is Abilities.KathScarletSVAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.KathScarletSEAbility());

                SEImageNumber = 151;
            }
        }
    }
}

namespace Abilities
{
    public class KathScarletSVAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += KathScarletSVPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= KathScarletSVPilotAbility;
        }

        private void KathScarletSVPilotAbility(ref int diceNumber)
        {
            if (Combat.ShotInfo.InArcByType(ArcTypes.RearAux))
            {
                Messages.ShowInfo("Defender is within auxiliary firing arc. Roll 1 additional attack die.");
                diceNumber++;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KathScarletSEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.ChosenWeapon != HostShip.PrimaryWeapon) return;

            if (Combat.Defender.ShipsBumped.Any(s => s.Owner.PlayerNo == HostShip.Owner.PlayerNo && !s.IsUnique))
            {
                Messages.ShowInfo("Kath Scarlet: +1 attack die");
                count++;
            }
        }
    }
}