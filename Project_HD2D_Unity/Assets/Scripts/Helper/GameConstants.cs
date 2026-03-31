public static class GameConstants
{
    public const float DEAD_STICK = 0.25f;
    public const float DEAD_STICK_SQUARE = 0.0625f; 
    public const float SNAP_BACK = 0.16f;
    
    public const float ANIM_WALK_THRESHOLD = 0.16f; 
    public const float ANIM_RUN_THRESHOLD = 0.8f;
    
    public const float GRAVITY_FORCE = 9.81f;
    public const string PLAYER_TAG = "Player";

    public const float PLAYER_ANIM_MAGNITUDE_RUN = 1f;
    public const float PLAYER_ANIM_MAGNITUDE_WALK = 0.5f;
    public const float PLAYER_ANIM_MAGNITUDE_IDLE = 0f;

    public const float VELOCITY_TO_SNAP_TO_0 = 0.1f;

    public const float CHECK_GROUND_RADIUS = 0.2f;
}