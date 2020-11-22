using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace EEMod
{
    public class ParticleZone
    {
        public ParticleZone(int MaxLength)
        {
            particles = new Particle[MaxLength];
        }

        internal readonly Particle[] particles;
        private List<IParticleModule> BaseZoneModules = new List<IParticleModule>();
        private readonly List<IParticleSpawner> SpawningModules = new List<IParticleSpawner>();
        public bool CanSpawn { get; set; }
        public int zoneTimer;
        private int MAXDRAWDISTANCE => 2000;

        public int SpawnParticles(Vector2 position, Vector2? velocity = null, Texture2D texture = null, int timeLeft = 60, int scale = 1, Color? colour = null, params IParticleModule[] CustomBaseZoneModule)
        {
            if (!Main.gamePaused)
            {
                if ((position - Main.LocalPlayer.Center).LengthSquared() < MAXDRAWDISTANCE * MAXDRAWDISTANCE)
                {
                    CanSpawn = false;
                    foreach (IParticleSpawner Module in SpawningModules)
                    {
                        Module.CanSpawn(this);
                    }

                    if (!CanSpawn) return -1;

                    for (int i = 0; i < particles.Length; i++)
                    {
                        if (particles[i] != null)
                        {
                            if (!particles[i].active)
                            {
                                particles[i] = new Particle(position, timeLeft, texture ?? Main.magicPixel, velocity, scale, colour ?? Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                                return i;
                            }
                        }
                        else
                        {
                            particles[i] = new Particle(position, timeLeft, texture ?? Main.magicPixel, velocity, scale, colour ?? Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public int SpawnParticles(Vector2 position, Vector2? velocity = null, params IParticleModule[] CustomBaseZoneModule)
        {
            if (!Main.gamePaused)
            {
                if ((position - Main.LocalPlayer.Center).LengthSquared() < MAXDRAWDISTANCE * MAXDRAWDISTANCE)
                {
                    CanSpawn = false;
                    foreach (IParticleSpawner Module in SpawningModules)
                    {
                        Module.CanSpawn(this);
                    }

                    if (!CanSpawn) return -1;

                    for (int i = 0; i < particles.Length; i++)
                    {
                        if (particles[i] != null)
                        {
                            if (!particles[i].active)
                            {
                                particles[i] = new Particle(position, 60, Main.magicPixel, Vector2.Zero, 1, Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                                return i;
                            }
                        }
                        else
                        {
                            particles[i] = new Particle(position, 60, Main.magicPixel, velocity, 1, Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public int SpawnParticles(Vector2 position, Vector2? velocity = null, int scale = 1, Color? colour = null, params IParticleModule[] CustomBaseZoneModule)
        {
            if (!Main.gamePaused)
            {
                if ((position - Main.LocalPlayer.Center).LengthSquared() < MAXDRAWDISTANCE * MAXDRAWDISTANCE)
                {
                    CanSpawn = false;
                    foreach (IParticleSpawner Module in SpawningModules)
                    {
                        Module.CanSpawn(this);
                    }

                    if (!CanSpawn) return -1;

                    for (int i = 0; i < particles.Length; i++)
                    {
                        if (particles[i] != null)
                        {
                            if (!particles[i].active)
                            {
                                particles[i] = new Particle(position, 60, Main.magicPixel, velocity, scale, colour ?? Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                                return i;
                            }
                        }
                        else
                        {
                            particles[i] = new Particle(position, 60, Main.magicPixel, velocity, scale, colour ?? Color.White, CustomBaseZoneModule.ToArray().Length == 0 ? BaseZoneModules.ToArray() : CustomBaseZoneModule);
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public int SpawnParticles(Vector2 position, Vector2? velocity = null, Color? colour = null, params IParticleModule[] CustomBaseZoneModule)
        {
            if (!Main.gamePaused)
            {
                if ((position - Main.LocalPlayer.Center).LengthSquared() < MAXDRAWDISTANCE * MAXDRAWDISTANCE)
                {
                    CanSpawn = false;
                    foreach (IParticleSpawner Module in SpawningModules)
                    {
                        Module.CanSpawn(this);
                    }

                    if (!CanSpawn) return -1;

                    for (int i = 0; i < particles.Length; i++)
                    {
                        if (particles[i] != null)
                        {
                            if (!particles[i].active)
                            {
                                particles[i] = new Particle(position, 60, Main.magicPixel, velocity, 1, colour ?? Color.White, CustomBaseZoneModule.ToArray() ?? BaseZoneModules.ToArray());
                                return i;
                            }
                        }
                        else
                        {
                            particles[i] = new Particle(position, 60, Main.magicPixel, velocity, 1, colour ?? Color.White, CustomBaseZoneModule.ToArray().Length == 0 ? BaseZoneModules.ToArray() : CustomBaseZoneModule);
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public void SetModules(params IParticleModule[] Module)
        {
            BaseZoneModules.Clear(); BaseZoneModules = Module.ToList();
        }

        public void SetSpawningModules(IParticleSpawner Module)
        {
            SpawningModules.Clear(); SpawningModules.Add(Module);
        }

        public void Update()
        {
            for (int k = 0; k < particles.Length; k++)
            {
                if (particles[k] != null)
                {
                    if (particles[k].active)
                    {
                        particles[k]?.Update();
                    }
                }
            }
            zoneTimer++;
        }

        public void Draw()
        {
            for (int k = 0; k < particles.Length; k++)
            {
                if (particles[k] != null)
                {
                    if (particles[k].active)
                    {
                        particles[k]?.Draw();
                    }
                }
            }
        }
    }

    internal class SpawnPeriodically : IParticleSpawner
    {
        private int spawnRate;

        public SpawnPeriodically(int spawnRate)
        {
            this.spawnRate = spawnRate;
        }

        public void CanSpawn(in ParticleZone pz)
        {
            if (pz.zoneTimer % spawnRate == 0)
                pz.CanSpawn = true;
        }
    }

    internal class SpawnRandomly : IParticleSpawner
    {
        private float chance;

        public SpawnRandomly(float chance)
        {
            this.chance = chance;
        }

        public void CanSpawn(in ParticleZone pz)
        {
            if (Main.rand.NextFloat(1f) < chance)
                pz.CanSpawn = true;
        }
    }

    public interface IParticleSpawner
    {
        void CanSpawn(in ParticleZone pz);
    }
}