public static class CheckpointManager
{
    private static PlayerState _lastCheckpoint;

    public static void Save(PlayerState state)
    {
        _lastCheckpoint = state;
    }

    public static PlayerState Load()
    {
        return _lastCheckpoint;
    }
}