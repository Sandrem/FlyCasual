using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Tokens;
using Bombs;

namespace UpgradesList.SecondEdition
{
    public class ConnerNets : GenericContactMineSE
    {
        public ConnerNets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Conner Nets",
                UpgradeType.Device,
                cost: 5,
                charges: 1,
                cannotBeRecharged: true,
                subType: UpgradeSubType.Mine,
                seImageNumber: 64
            );

            bombPrefabPath = "Prefabs/Bombs/ConnerNets";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            ship.Damage.TryResolveDamage(
                1, 
                new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                },
                delegate { AssignIonTokens(ship); }
            );
        }

        private void AssignIonTokens(GenericShip ship)
        {
            Messages.ShowInfo("Conner Nets: " + ship.PilotInfo.PilotName + " suffers damage and gets ion tokens");

            ship.Tokens.AssignTokens(
                delegate { return CreateIonToken(ship); },
                3,
                Triggers.FinishTrigger
            );
        }

        private GenericToken CreateIonToken(GenericShip ship)
        {
            return new IonToken(ship);
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { callBack(); });
        }
    }
}