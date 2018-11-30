using Arcs;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class KathScarletScum : Firespray31
        {
            public KathScarletScum() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kath Scarlet",
                    7,
                    38,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KathScarletScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Illicit },
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Kath Scarlet";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KathScarletScumAbility : GenericAbility
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
            if (Combat.ShotInfo.InArcByType(ArcType.RearAux))
            {
                Messages.ShowInfo("Defender is within auxiliary firing arc. Roll 1 additional attack die.");
                diceNumber++;
            }
        }
    }
}