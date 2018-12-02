using BoardTools;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class Dengar : JumpMaster5000
        {
            public Dengar() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dengar",
                    6,
                    64,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DengarPilotAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 214
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DengarPilotAbility : Abilities.FirstEdition.DengarPilotAbility
    {
        protected override bool CanCounterattackUsingShotInfo(ShotInfo counterAttackInfo)
        {
            return counterAttackInfo.InArc && HostShip.ArcsInfo.GetArc<Arcs.ArcMobile>().Facing == Arcs.ArcFacing.Forward;
        }

        protected override bool CanUseAbility()
        {
            return HostShip.State.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}