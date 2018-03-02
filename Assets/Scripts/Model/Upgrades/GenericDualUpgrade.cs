using System;
using Ship;
using UnityEngine;
using System.Linq;
using ActionsList;

namespace Upgrade
{
    public class GenericDualUpgrade : GenericUpgrade
    {
        protected Type AnotherSide { get; set; }
        private GenericDualUpgrade AnotherSideInstance { get; set; }

        public void Flip()
        {
            Discard(SetAnotherSide);
        }

        private void SetAnotherSide()
        {
            if (AnotherSideInstance == null) CreateAnotherSideInstance();

            ReplaceUpgradeBy(AnotherSideInstance);
        }

        private void CreateAnotherSideInstance()
        {
            AnotherSideInstance = (GenericDualUpgrade) Activator.CreateInstance(AnotherSide);
        }
    }
}
