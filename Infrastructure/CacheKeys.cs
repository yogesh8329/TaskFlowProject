namespace TaskFlow.Api.Common
{
    public static class CacheKeys
    {
        public static string UserTasks(int userId)
            => $"tasks:user:{userId}:all";
    }
}
