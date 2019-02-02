using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class VeteranInstincts : GenericUpgrade, IModifyPilotSkill
    {
        public VeteranInstincts() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Veteran Instincts",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.VeteranInstinctsAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(56, 0));
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            host.State.AddPilotSkillModifier(this);
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            HostShip.State.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill += 2;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class VeteranInstinctsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Roster.UpdateShipStats(HostShip);
        }

        public override void DeactivateAbility()
        {
            Roster.UpdateShipStats(HostShip);
        }
    }
}