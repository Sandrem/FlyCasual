using Actions;
using ActionsList;
using Arcs;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonCannonTurret : GenericSpecialWeapon
    {
        public IonCannonTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon Turret",
                UpgradeType.Turret,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.SingleTurret
                ),
                addArc: new ShipArcInfo(ArcType.SingleTurret),
                addAction: new ActionInfo(typeof(RotateArcAction)),
                abilityType: typeof(Abilities.FirstEdition.IonDamageAbility),
                seImageNumber: 32
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonDamageAbilityTurret : Abilities.FirstEdition.IonDamageAbility
    {
        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new RotateArcAction(), HostUpgrade);
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(RotateArcAction), HostUpgrade);
        }

        protected override void IonTurretEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new IonToken(Combat.Defender),
                ionTokens,
                delegate
                {
                    GameManagerScript.Wait(2, DefenderSuffersDamage);
                }
            );
        }
    }
}