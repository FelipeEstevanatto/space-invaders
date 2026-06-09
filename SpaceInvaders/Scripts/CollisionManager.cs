using System;
using System.Collections.Generic;
using SpaceInvaders;

namespace SpaceInvaders
{
    public static class CollisionManager
    {
        public static void Resolve(
            PlayerShip player,
            List<Alien> aliens,
            List<Shield> shields,
            List<Projectile> projectiles,
            Action<Alien> alienDestroyed,
            Action playerHit)
        {
            ResolvePlayerProjectileAlienCollisions(
                aliens,
                projectiles,
                alienDestroyed
            );

            ResolveAlienProjectilePlayerCollisions(
                player,
                projectiles,
                playerHit
            );
            ResolveProjectileShieldCollisions(
                shields,
                projectiles
            );
        }

        private static void ResolvePlayerProjectileAlienCollisions(
            List<Alien> aliens,
            List<Projectile> projectiles,
            Action<Alien> alienDestroyed)
        {
            foreach (Projectile projectile in projectiles)
            {
                if (!projectile.IsActive ||
                    projectile.Owner != ProjectileOwner.Player)
                {
                    continue;
                }

                foreach (Alien alien in aliens)
                {
                    if (!alien.IsActive)
                    {
                        continue;
                    }

                    if (projectile.Bounds.IntersectsWith(alien.Bounds))
                    {
                        projectile.Deactivate();
                        alien.Destroy();
                        alienDestroyed(alien);
                        break;
                    }
                }
            }
        }

        private static void ResolveAlienProjectilePlayerCollisions(
            PlayerShip player,
            List<Projectile> projectiles,
            Action playerHit)
        {
            if (player.IsInvulnerable) return;

            foreach (Projectile projectile in projectiles)
            {
                if (!projectile.IsActive ||
                    projectile.Owner != ProjectileOwner.Alien)
                {
                    continue;
                }

                if (projectile.Bounds.IntersectsWith(player.Bounds))
                {
                    projectile.Deactivate();
                    playerHit();
                    break;
                }
            }
        }
        private static void ResolveProjectileShieldCollisions(
            List<Shield> shields,
            List<Projectile> projectiles)
        {
            foreach (Projectile projectile in projectiles)
            {
                if (!projectile.IsActive) continue;

                foreach (Shield shield in shields)
                {
                    if (!shield.IsActive) continue;

                    if (projectile.Bounds.IntersectsWith(shield.Bounds))
                    {
                        projectile.Deactivate(); // Destroy the bullet
                        shield.TakeDamage();     // Hurt the shield
                        break;
                    }
                }
            }
        }
    }
}