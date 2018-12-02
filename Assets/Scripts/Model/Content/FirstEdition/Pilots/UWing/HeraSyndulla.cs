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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(PhoenixSquadronMod));
                ImageUrl = "https://i.imgur.com/gcK261Z.png";
            }
        }
    }
}