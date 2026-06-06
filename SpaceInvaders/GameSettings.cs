namespace SpaceInvaders
{
    public static class GameSettings
    {
        public const int TimerIntervalMs = 16; // Aproximadamente 60 FPS

        public const int InitialLives = 3;

        public const int PlayerShootCooldown = 15;

        public const int AlienBaseSpeed = 2;
        public const int AlienMaxSpeed = 12;
        public const int AlienDropDistance = 18;

        public const int AlienRows = 4;
        public const int AlienColumns = 8;
        public const int AlienSpacingX = 14;
        public const int AlienSpacingY = 14;

        public const int InitialAlienShootCooldown = 60;
        public const int AlienShootCooldownMin = 25;
        public const int AlienShootCooldownMax = 90;
    }
}