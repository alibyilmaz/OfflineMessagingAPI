
using Castle.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfflineMessagingAPI.Interfaces;
using OfflineMessagingAPI.Models;
using OfflineMessagingAPI.Services;
using OfflineMessagingAPI.Settings;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace OfflineMessagingAPI.UnitTest
{
    [TestClass]
    public class AuthTest
    {

        private IUserService _userService;
        private IBlockService _blockService;

        

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
            _blockService = new BlockService(settings);

        }

        [TestMethod]
        public void Register_Should_Insert_New_User()
        {
            var user = new User
            {
                Username = "Test User",
                Password = "testPassword",
            };
            var inserted = _userService.Register(user);
            Assert.AreNotEqual(0, inserted.Id);
            Assert.AreEqual(24, inserted.Id.ToString().Length);
        }
        [TestMethod]
        public void BlockUser_Should_Block_User()
        {

            var user = new User
            {
                Guid = Guid.NewGuid(),
                Username = "Test User",
                Password = "testPassword",
            };
            var inserted = _userService.Register(user);
            var userFromToken = new User
            {
                Guid = Guid.NewGuid(),
                Username = "Test User From Token",
                Password = "testPassword",
            };
            var insertedFromToken = _userService.Register(userFromToken);
            var userBlock = new User
            {

                Guid = Guid.NewGuid(),
                Username = "Test User Block",
                Password = "testPassword",
            };
            var insertedBlock = _userService.Register(userBlock);
            var blocked = _blockService.BlockUser(new BlockUsers
            {
                BlockFromUserID = inserted.Result.Guid,
                BlockToUserID = insertedBlock.Result.Guid
            });
            Assert.AreEqual(true, blocked);
        }
    }
}
