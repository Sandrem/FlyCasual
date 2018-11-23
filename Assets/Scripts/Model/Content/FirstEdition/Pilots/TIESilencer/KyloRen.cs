using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIESilencer
    {
        public class KyloRen : TIESilencer
        {
            public KyloRen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kylo Ren",
                    9,
                    35,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KyloRenPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}