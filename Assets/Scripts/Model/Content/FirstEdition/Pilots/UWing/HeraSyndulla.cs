using Mods.ModsList;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.UWing
    {
        public class HeraSyndulla : UWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    7,
                    28,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                RequiredMods.Add(typeof(PhoenixSquadronMod));
            }
        }
    }
}