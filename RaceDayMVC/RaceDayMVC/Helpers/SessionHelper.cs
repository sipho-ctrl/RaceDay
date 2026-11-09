namespace RaceDayMVC.Helpers
{
    public static class SessionHelper
    {
        public const string TokenKey = "JwtToken";
        public const string RoleKey = "UserRole";
        public const string UsernameKey = "Username";
        public const string UserIdKey = "UserId";

        public static string? GetToken(ISession session) => session.GetString(TokenKey);
        public static string? GetRole(ISession session) => session.GetString(RoleKey);
        public static string? GetUsername(ISession session) => session.GetString(UsernameKey);
        public static int? GetUserId(ISession session)
        {
            var value = session.GetString(UserIdKey);
            return int.TryParse(value, out var id) ? id : null;
        }

        public static void SetUser(ISession session, string token, string role, string username, int userId)
        {
            session.SetString(TokenKey, token);
            session.SetString(RoleKey, role);
            session.SetString(UsernameKey, username);
            session.SetString(UserIdKey, userId.ToString());
        }

        public static void Clear(ISession session) => session.Clear();
    }
}