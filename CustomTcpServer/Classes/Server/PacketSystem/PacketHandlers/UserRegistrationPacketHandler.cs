using InfinityServer.Classes.Utils;
using Newtonsoft.Json;
using System.Text;
using System;
using InfinityServer.App;
using InfinityServer.Classes.Server.User;
using InfinityServer.Classes.Server.Security;

namespace InfinityServer.Classes.Server.PacketSystem.PacketHandlers
{
    public class UserRegistrationPacketHandler : IPacketHandler
    {
        private PacketHandler _serverPacketHandler;

        public async void Handle(Packet packet, ClientHandler clientHandler)
        {
            _serverPacketHandler = clientHandler.GetInfinityTcpServer.GetServerPacketHandler;

            // Convert bytes to string
            string jsonString = Encoding.UTF8.GetString(packet.Data);

            string[] splitJsonString = jsonString.Split(new[] { "-newpacket-" }, StringSplitOptions.None);

            string userDetailsJsonString = splitJsonString[0].Trim();
            string userAuthDeatilsJsonString = splitJsonString[1].Trim();

            // Deserialize the JSON string to UserAuthDetails object
            UserDetails userDetails = JsonConvert.DeserializeObject<UserDetails>(userDetailsJsonString);
            UserAuthDetails userAuthDetails = JsonConvert.DeserializeObject<UserAuthDetails>(userAuthDeatilsJsonString);

            if (!EmailValidator.IsValidEmail(userDetails.Email))
            {
                // Send a response packet saying must enter a valid email
                byte[] mustEnterValidEmailBytes = Encoding.UTF8.GetBytes($"You must enter a valid email.");
                await _serverPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, mustEnterValidEmailBytes, "Must Enter Valid Email", clientHandler.ClientGuid.ToString(), true);

                return;
            }

            if (await InfinityApplication.Instance.Database.DoesUsernameExist(userDetails.Username))
            {
                // Send a response packet saying username already exist
                byte[] usernameAlreadyExistBytes = Encoding.UTF8.GetBytes($"{userDetails.Username} is already in use, Please try a different username.");
                await _serverPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, usernameAlreadyExistBytes, "Username Already Exist", clientHandler.ClientGuid.ToString(), true);
                return;
            }

            if (await InfinityApplication.Instance.Database.DoesEmailExist(userDetails.Email))
            {
                // Send a response packet saying email already exist
                byte[] emailAlreadyExistBytes = Encoding.UTF8.GetBytes($"{userDetails.Email} is already in use, Please try a different email.");
                await _serverPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, emailAlreadyExistBytes, "Email Already Exist", clientHandler.ClientGuid.ToString(), true);
                return;
            }

            if (await AuthenticationService.RegisterUser(userDetails, userAuthDetails.PasswordHash))
            {
                // Send a response packet letting the user know they successfully registered
                byte[] registrationSuccessfulBytes = Encoding.UTF8.GetBytes("Registration Successful");
                await _serverPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, registrationSuccessfulBytes, "Registration Response", clientHandler.ClientGuid.ToString(), true);
            }
            else
            {
                // Send a response packet letting the user know the registration failed
                byte[] registrationFailedBytes = Encoding.UTF8.GetBytes("Registration Failed");
                await _serverPacketHandler.CreateAndSendPacketAsync(clientHandler.GetInfinityTcpServer, registrationFailedBytes, "Registration Response", clientHandler.ClientGuid.ToString(), true);
            }
        }
    }
}