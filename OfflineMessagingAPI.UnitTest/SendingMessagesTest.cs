using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMessagingAPI.Interfaces;
using OfflineMessagingAPI.Models;
using OfflineMessagingAPI.Services;
using OfflineMessagingAPI.Settings;
using System.Configuration;
using System.Drawing;
using Microsoft.Extensions.Configuration;

namespace OfflineMessagingAPI.UnitTest
{
    [TestClass]
    public class SendingMessagesTest
    {
        private IJwtManagerService _jwtManagerService;
        private IConfiguration _configuration;
        private IUserService _userService;
        private IMessageService _messageService;


        [TestInitialize]
        public void Initialize()
        {
            IMongoSettings settings = new MongoSettings();
            {
                settings.ConnectionString = "mongodb://localhost:27017";
                settings.Database = "offlineMessagingDb";
                settings.UserCollectionName = "users";
            }
            _userService = new UserService(settings);
            _messageService = new MessageService(settings);
            _jwtManagerService = new JwtManagerService(_configuration, _userService);
        }
        [TestMethod]
        public void Validate_Login_Credentials()
        {
            // Arrange
            var login = new User();
            login.Username = "admin";
            login.Password = "admin";
            // Act
            var token = _jwtManagerService.Authenticate(login);
            // Assert
            Assert.IsTrue(token != null);
        }
        [TestMethod]
        public void Send_Message_To_User()
        {
            // Arrange
            var login = new User();
            login.Username = "admin";
            login.Password = "admin";

            var user = _userService.GetUserByUserName(login.Username);
            DateTime validTo = user.Result.Select(x => x.ValidTo).FirstOrDefault();
            Assert.IsTrue(validTo > DateTime.Now);
            var message = new Messages();
           
            message.SenderUser = user.Result.Select(x => x.Username).FirstOrDefault();
            message.ReceiverUser = "user1";
            message.Message = "Hello World";
            // Act
            var result = _messageService.SendMessageByUserName(message, message.ReceiverUser);
            // Assert
            Assert.IsTrue(result.Result.Message == "Message sent");
        }
    }
}
