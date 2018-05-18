using Arcs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KathScarletSV : Firespray31
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