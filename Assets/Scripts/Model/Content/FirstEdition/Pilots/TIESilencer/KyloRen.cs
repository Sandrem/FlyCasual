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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KyloRenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}