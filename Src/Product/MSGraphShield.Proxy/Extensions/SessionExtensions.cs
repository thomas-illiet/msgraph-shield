using Titanium.Web.Proxy.EventArguments;

namespace MSGraphShield.Proxy.Extensions
{
    internal static class SessionExtensions
    {
        /// <summary>
        /// Determines if the session is a Microsoft Graph API request.
        /// </summary>
        /// <param name="session">The session.</param>
        public static bool IsGraphRequest(this SessionEventArgs session) =>
            session.HttpClient.Request.RequestUri.Host.Contains("graph.microsoft.", StringComparison.OrdinalIgnoreCase) ||
            session.HttpClient.Request.RequestUri.Host.Contains("microsoftgraph.", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the data associated with the specified key from the session.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key of the data.</param>
        public static T GetData<T>(this SessionEventArgs session, string key)
        {
            var dataStore = (Dictionary<string, object>) session.ClientUserData;
            if (dataStore != null)
            {
                if (dataStore.TryGetValue(key, out var value))
                    return (T)value;
            }

            return default!;
        }

        /// <summary>
        /// Inserts or updates the data associated with the specified key in the session.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key of the data.</param>
        /// <param name="value">The data to be inserted or updated.</param>
        public static void InsertData<T>(this SessionEventArgs session, string key, T value)
        {
            if (session.ClientUserData == null) 
                session.ClientUserData = new Dictionary<string, object>();
            var dataStore = (Dictionary<string, object>?)session.ClientUserData;


            if (session.IsExist(key) == false)
            {
                dataStore!.Add(key, value);
            }
            else
            {
                dataStore.Remove(key);
                dataStore.Add(key, value);
            }
        }

        /// <summary>
        /// Checks if the session contains data associated with the specified key.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="key">The key to check.</param>
        private static bool IsExist(this SessionEventArgs session, string key)
        {
            var dataStore = (Dictionary<string, object>)session.ClientUserData;
            return dataStore.ContainsKey(key);
        }
           
    }
}
