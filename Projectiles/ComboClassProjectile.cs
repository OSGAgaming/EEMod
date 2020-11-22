using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace EEMod.Projectiles
{
    public delegate void Combo();

    public abstract class ComboWeapon : ModProjectile, IComboProjectile
    {
        protected float progression => projOwner.itemAnimation / (float)projOwner.itemAnimationMax;
        protected bool isFinished => progression >= 1;
        protected Player projOwner => Main.player[projectile.owner];

        private int ComboSelection;

        public void SetCombo(int CurrentCombo) => ComboSelection = CurrentCombo;

        private readonly Dictionary<int, Combo> Combos = new Dictionary<int, Combo>();

        protected void AddCombo(int key, Combo combo)
        {
            if (!Combos.ContainsKey(key))
                Combos.Add(key, combo);
        }

        public override void AI()
        {
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Combos[ComboSelection].Invoke();
        }
    }
}