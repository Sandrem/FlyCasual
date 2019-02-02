using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class IonBombs : GenericTimedBomb
    {
        GenericShip _ship = null;

        public IonBombs() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Bombs",
                type: UpgradeType.Bomb,
                cost: 2
            );

            bombPrefabPath = "Prefabs/Bombs/IonBomb";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            _ship = ship;

            ship.AssignedDamageDiceroll.AddDice(DieSide.Unknown);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Assign Ion Tokens From Bomb",
                TriggerType = TriggerTypes.OnTokenIsAssigned,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = SufferIonBombTokens,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, Triggers.FinishTrigger);
        }

        private void SufferIonBombTokens(object sender, EventArgs e)
        {
            _ship.Tokens.AssignToken(typeof(IonToken), SufferSecondIonBombToken);
        }

        private void SufferSecondIonBombToken()
        {
            Messages.ShowInfoToHuman(string.Format("{0}: Dealt second ion token to {1}", UpgradeInfo.Name, _ship.PilotInfo.PilotName));
            _ship.Tokens.AssignToken(typeof(IonToken), Triggers.FinishTrigger);
        }

        private void PlayDefferedSound(GameObject bombObject, Action callBack)
        {
            Sounds.PlayBombSound(bombObject, "IonBomb");
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }
    }
}