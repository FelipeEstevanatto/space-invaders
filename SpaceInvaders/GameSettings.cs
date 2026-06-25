using System;
using System.Drawing;

namespace SpaceInvaders
{
    public static class GameSettings
    {
        static GameSettings()
        {
            if (TimerIntervalMs <= 0)
            {
                throw CreateConfigurationException(nameof(TimerIntervalMs), "must be greater than zero.");
            }

            if (InitialLives <= 0)
            {
                throw CreateConfigurationException(nameof(InitialLives), "must be greater than zero.");
            }

            if (MaxLevel <= 0)
            {
                throw CreateConfigurationException(nameof(MaxLevel), "must be greater than zero.");
            }

            if (PlayerSpeed <= 0)
            {
                throw CreateConfigurationException(nameof(PlayerSpeed), "must be greater than zero.");
            }

            if (PlayerShootCooldown < 0)
            {
                throw CreateConfigurationException(nameof(PlayerShootCooldown), "must be zero or greater.");
            }

            if (ShieldCount <= 0)
            {
                throw CreateConfigurationException(nameof(ShieldCount), "must be greater than zero.");
            }

            if (AlienBaseSpeed <= 0)
            {
                throw CreateConfigurationException(nameof(AlienBaseSpeed), "must be greater than zero.");
            }

            if (AlienMaxSpeed < AlienBaseSpeed)
            {
                throw CreateConfigurationException(
                    nameof(AlienMaxSpeed),
                    $"({AlienMaxSpeed}) must be greater than or equal to {nameof(AlienBaseSpeed)} ({AlienBaseSpeed}).");
            }

            if (AlienDropDistance <= 0)
            {
                throw CreateConfigurationException(nameof(AlienDropDistance), "must be greater than zero.");
            }

            if (AlienColumns <= 0)
            {
                throw CreateConfigurationException(nameof(AlienColumns), "must be greater than zero.");
            }

            if (AlienSpacingX < 0 || AlienSpacingY < 0)
            {
                throw CreateConfigurationException(
                    "Alien spacing",
                    $"{nameof(AlienSpacingX)} and {nameof(AlienSpacingY)} must be zero or greater.");
            }

            if (AlienShootCooldownMin > AlienShootCooldownMax)
            {
                throw CreateConfigurationException(
                    nameof(AlienShootCooldownMin),
                    $"({AlienShootCooldownMin}) must be less than or equal to {nameof(AlienShootCooldownMax)} ({AlienShootCooldownMax}).");
            }

            if (InitialAlienShootCooldown < 0)
            {
                throw CreateConfigurationException(nameof(InitialAlienShootCooldown), "must be zero or greater.");
            }

            if (AlienRowColors.Length == 0 || AlienRowPoints.Length == 0)
            {
                throw CreateConfigurationException(
                    "alien row configuration",
                    $"{nameof(AlienRowColors)} and {nameof(AlienRowPoints)} must contain at least one entry.");
            }

            if (AlienRowColors.Length != AlienRowPoints.Length)
            {
                throw CreateConfigurationException(
                    "alien row configuration",
                    $"{nameof(AlienRowColors)} and {nameof(AlienRowPoints)} must have the same length.");
            }

            if (AlienRows < 0)
            {
                throw CreateConfigurationException(nameof(AlienRows), "must be zero or greater.");
            }
        }

        public const int TimerIntervalMs = 16; // Aproximadamente 60 FPS

        public const int InitialLives = 3;
        public const int MaxLevel = 3;

        public const int PlayerShootCooldown = 15;
        public const int PlayerSpeed = 6;
        public const int ShieldCount = 3;

        public const int AlienBaseSpeed = 2;
        public const int AlienMaxSpeed = 10;
        public const int AlienDropDistance = 18;

        // Valores menores que AlienRowColors ignoram entradas extras; maiores repetem a última.
        public const int AlienRows = 4;

        public const int AlienColumns = 8;
        public const int AlienSpacingX = 14;
        public const int AlienSpacingY = 14;

        public const int InitialAlienShootCooldown = 60;
        public const int AlienShootCooldownMin = 25;
        public const int AlienShootCooldownMax = 90;

        public static readonly Color[] AlienRowColors =
        {
            Color.Red,          // row 0: back/top row
            Color.Orange,
            Color.Yellow,
            Color.LimeGreen    // row 3: front/bottom row
        };

        public static readonly int[] AlienRowPoints =
        {
            40,     // row 0: back/top row
            30,
            20,
            10      // row 3: front/bottom row
        };

        public static int EffectiveAlienRows =>
            AlienRows > 0 ? AlienRows : AlienRowColors.Length;

        public static int ResolveAlienRowIndex(int rowIndex)
        {
            int lastIndex = AlienRowColors.Length - 1;

            if (rowIndex < 0)
            {
                return 0;
            }

            return rowIndex <= lastIndex ? rowIndex : lastIndex;
        }

        public static Color GetAlienRowColor(int rowIndex) =>
            AlienRowColors[ResolveAlienRowIndex(rowIndex)];

        public static int GetAlienRowPointValue(int rowIndex) =>
            AlienRowPoints[ResolveAlienRowIndex(rowIndex)];

        public static void ValidateLayout(Size viewport)
        {
            if (viewport.Width <= 0 || viewport.Height <= 0)
            {
                throw CreateConfigurationException(
                    "viewport",
                    "width and height must be greater than zero.");
            }

            const int horizontalMargin = 40;
            int totalWidth = AlienColumns * Alien.DefaultWidth + (AlienColumns - 1) * AlienSpacingX;

            if (totalWidth > viewport.Width - horizontalMargin)
            {
                throw CreateConfigurationException(
                    "alien grid",
                    $"width ({totalWidth}px) exceeds the viewport ({viewport.Width}px) with margins. " +
                    $"Reduce {nameof(AlienColumns)} or {nameof(AlienSpacingX)}, or increase the viewport size.");
            }
        }

        private static InvalidOperationException CreateConfigurationException(string setting, string message)
        {
            return new InvalidOperationException($"Invalid GameSettings: {setting} {message}");
        }
    }
}
