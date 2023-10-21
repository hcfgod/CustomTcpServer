using InfinityServer.App;
using InfinityServer.Classes.Server.User;
using InfinityServer.Classes.Utils;
using Serilog;
using System;
using System.Threading.Tasks;

namespace InfinityServer.Classes.Server.Security
{
    public static class AuthenticationService
    {
        public static async Task<bool> RegisterUser(UserDetails userDetails, string clientHashedPassword)
        {
            try
            {
                UserDetails existingUser = await InfinityApplication.Instance.Database.GetUserDetailsByEmail(userDetails.Email);

                if (existingUser != null)
                {
                    // User with the same email already exists.
                    // Send a user already exist packet
                    Log.Logger.Warning("(ClientHandler) AuthenticateUser - Email already exists.");
                    return false;
                }

                string salt = PasswordHelper.CreateSalt();
                string serverHashedPassword = PasswordHelper.HashPassword(clientHashedPassword, salt); // makes it a double hashed password that is salted

                UserAuthDetails userAuth = new UserAuthDetails
                {
                    UserID = userDetails.UserID,
                    Username = userDetails.Username,
                    PasswordHash = serverHashedPassword,
                    PasswordSalt = salt
                };

                await InfinityApplication.Instance.Database.AddUserDetails(userDetails);
                await InfinityApplication.Instance.Database.AddUserAuthentication(userAuth);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"(ClientHandler) RegisterUser - Error registering user: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> AuthenticateUser(string username, string clientHashedPassword)
        {
            try
            {
                UserDetails userDetails = await InfinityApplication.Instance.Database.GetUserDetailsByUsername(username);

                if (userDetails == null)
                {
                    // No such user exists.
                    //Send a Wrong user/password response packet
                    InfinityApplication.Instance.Logger.Error("(ClientHandler) AuthenticateUser - No Such User.");
                    return false;
                }

                UserAuthDetails userAuth = await InfinityApplication.Instance.Database.GetUserAuthenticationByUserID(userDetails.UserID);

                string serverHashedPassword = PasswordHelper.HashPassword(clientHashedPassword, userAuth.PasswordSalt);

                if (serverHashedPassword == userAuth.PasswordHash)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"(ClientHandler) AuthenticateUser - Error authenticating user: {ex.Message}");
                return false;
            }
        }
    }
}
