using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;

namespace SpaceInvaders
{
    public sealed class AudioManager : IDisposable
    {
        private readonly MediaPlayer musicPlayer;
        private readonly Dictionary<SoundEffectType, Queue<MediaPlayer>> effectPools;
        private readonly Dictionary<SoundEffectType, Uri> effectUris;

        private bool isMuted;
        private bool isDisposed;

        public double MusicVolume { get; set; } = 0.2;
        public double EffectsVolume { get; set; } = 0.7;

        public bool IsMuted
        {
            get { return isMuted; }
        }

        public AudioManager()
        {
            musicPlayer = new MediaPlayer();
            musicPlayer.MediaEnded += MusicPlayer_MediaEnded;

            effectPools = new Dictionary<SoundEffectType, Queue<MediaPlayer>>();
            effectUris = new Dictionary<SoundEffectType, Uri>();
        }

        public void LoadEffects()
        {
            if (effectPools.Count > 0)
            {
                return;
            }

            LoadEffect(SoundEffectType.Shoot, "shoot.wav", 8);
            LoadEffect(SoundEffectType.AlienDestroyed, "explosion.wav", 8);
            LoadEffect(SoundEffectType.PlayerHit, "explosion.wav", 4);
            LoadEffect(SoundEffectType.GameOver, "explosion.wav", 2);
        }

        public void PlayMusic(string fileName)
        {
            if (isDisposed)
            {
                return;
            }

            Uri uri = GetSoundUri(fileName);

            musicPlayer.Open(uri);
            musicPlayer.Volume = isMuted ? 0 : MusicVolume;
            musicPlayer.Play();
        }

        public void PlayEffect(SoundEffectType effectType)
        {
            if (isDisposed || isMuted)
            {
                return;
            }

            if (!effectPools.ContainsKey(effectType))
            {
                return;
            }

            Queue<MediaPlayer> pool = effectPools[effectType];

            if (pool.Count == 0)
            {
                return;
            }

            MediaPlayer player = pool.Dequeue();

            player.Stop();
            player.Position = TimeSpan.Zero;
            player.Volume = EffectsVolume;
            player.Play();

            pool.Enqueue(player);
        }

        public bool ToggleMute()
        {
            isMuted = !isMuted;

            if (isMuted)
            {
                musicPlayer.Pause();
            }
            else
            {
                musicPlayer.Volume = MusicVolume;
                musicPlayer.Play();
            }

            return isMuted;
        }

        public void SetMusicVolume(double volume)
        {
            MusicVolume = ClampVolume(volume);
            musicPlayer.Volume = isMuted ? 0 : MusicVolume;
        }

        public void SetEffectsVolume(double volume)
        {
            EffectsVolume = ClampVolume(volume);
        }

        private void LoadEffect(
            SoundEffectType effectType,
            string fileName,
            int poolSize)
        {
            Uri uri = GetSoundUri(fileName);

            effectUris[effectType] = uri;

            Queue<MediaPlayer> pool = new Queue<MediaPlayer>();

            for (int i = 0; i < poolSize; i++)
            {
                MediaPlayer player = new MediaPlayer();
                player.Open(uri);
                player.Volume = EffectsVolume;
                pool.Enqueue(player);
            }

            effectPools[effectType] = pool;
        }

        private void MusicPlayer_MediaEnded(object sender, EventArgs e)
        {
            musicPlayer.Position = TimeSpan.Zero;

            if (!isMuted)
            {
                musicPlayer.Play();
            }
        }

        private static Uri GetSoundUri(string fileName)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "Resources",
                fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Sound file not found: " + path);
            }

            return new Uri(path);
        }

        private static double ClampVolume(double volume)
        {
            if (volume < 0)
            {
                return 0;
            }

            if (volume > 1)
            {
                return 1;
            }

            return volume;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            musicPlayer.Stop();
            musicPlayer.Close();

            foreach (Queue<MediaPlayer> pool in effectPools.Values)
            {
                foreach (MediaPlayer player in pool)
                {
                    player.Stop();
                    player.Close();
                }
            }

            isDisposed = true;
        }
    }
}