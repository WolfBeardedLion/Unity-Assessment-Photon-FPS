using UnityEngine;


    public class AsteroidsGame
    {
        public const float ASTEROIDS_MIN_SPAWN_TIME = 5.0f;
        public const float ASTEROIDS_MAX_SPAWN_TIME = 10.0f;

        public const float PLAYER_RESPAWN_TIME = 4.0f;

        public const int PLAYER_MAX_LIVES = 3;

        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return Color.green;
                case 1: return Color.red;
                case 2: return new Color(255, 0, 222);
                case 3: return new Color(255, 185, 0);
                case 4: return new Color(0, 217, 255);
                case 5: return new Color(0, 42, 255);
                case 6: return new Color(0, 255, 202);
                case 7: return new Color(215, 255, 0);
                case 8: return new Color(119, 0, 255);
                case 9: return new Color(226, 115, 0);
        }

            return Color.black;
        }
    }