using Arcs;
using Movement;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class OnyxSquadronSentry : TIERbHeavy
        {
            public OnyxSquadronSentry() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Sentry",
                    3,
                    33,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/3d/7c/3d7ca1c1-9e57-4085-a9a6-e82a3d92c6df/swz67_onyx-sentry.png";
            }
        }
    }
}