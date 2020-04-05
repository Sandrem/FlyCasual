using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;
using Remote;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class IonBombs : GenericTimedBombSE
    {
        GenericShip _ship = null;

        public IonBombs() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Bombs",
                type: UpgradeType.Device,
                cost: 6,
                charges: 2,
                subType: UpgradeSubType.Bomb
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e4c43791c16aea639f2e811c16d1dbcf.png";

            bombPrefabPath = "Prefabs/Bombs/IonBomb";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Messages.ShowInfo("Ion Bombs: 3 ion tokens are assigned to " + ship.PilotInfo.PilotName);

            ship.Tokens.AssignTokens(
                delegate { return new IonToken(ship); },
                3,
                delegate { CheckDamageToRemote(ship, callBack); }
            );
        }

        private void CheckDamageToRemote(GenericShip ship, Action callBack)
        {
            if (ship is GenericRemote)
            {
                Messages.ShowInfo("Ion Bombs: " + ship.PilotInfo.PilotName + " suffers damage");
                DealDamageToRemote(ship, callBack);
            }
            else
            {
                callBack();
            }
        }

        private void DealDamageToRemote(GenericShip ship, Action callBack)
        {
            _ship = ship;

            DamageSourceEventArgs damage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.BombDetonation
            };

            _ship.Damage.TryResolveDamage(1, 0, damage, callBack);
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            BombsManager.CurrentDevice = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }
    }
}