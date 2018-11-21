using Arcs;
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
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KathScarletScumAbility)
                );

                ModelInfo.SkinName = "Kath Scarlet";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
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
            if (Combat.ShotInfo.InArcByType(ArcTypes.RearAux))
            {
                Messages.ShowInfo("Defender is within auxiliary firing arc. Roll 1 additional attack die.");
                diceNumber++;
            }
        }
    }
}