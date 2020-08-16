using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Users.Contracts;
using Users.Repository.Entities;
using Users.Tests.Helpers;
using Xunit;

namespace Users.Tests
{
    public class UsersTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly string _path = "/api/users";

        public UsersTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _server.Dispose();
            _client.Dispose();
        }

        [Fact]
        public async Task NoDuplicateUsernamesAllowedAsync()
        {
            var username = "NoDuplicateAddTest";
            var addedResult = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = username}));
            addedResult.EnsureSuccessStatusCode();
            var result = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = username}));
            Assert.True(result.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UserNotFoundOnUpdateNonExistentUserAsync()
        {
            var result = await _client.PutAsync(_path + "/1",
                ContentHelper.GetStringContent(new UserContract {Username = "randomName"}));
            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UsernameShouldUpdateAsync()
        {
            var username = "calvin";

            var result = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = "usernameToBeUpdated"}));
            result.EnsureSuccessStatusCode();
            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
            var result2 = await _client.PutAsync(_path + "/" + user.Id,
                ContentHelper.GetStringContent(new UserContract {Username = username}));
            result2.EnsureSuccessStatusCode();

            user = JsonConvert.DeserializeObject<User>(await result2.Content.ReadAsStringAsync());

            Assert.Equal(username, user.Username);
        }

        [Fact]
        public async Task UsernameShouldNotUpdateIfNotUniqueAsync()
        {
            var username = "calvin2";

            var result = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = "usernameNotUnique"}));
            result.EnsureSuccessStatusCode();

            var result2 = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = username}));
            result2.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
            var result3 = await _client.PutAsync(_path + "/" + user.Id,
                ContentHelper.GetStringContent(new UserContract {Username = username}));

            Assert.True(result3.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenDeleteOnNonExistentUserAsync()
        {
            var result = await _client.DeleteAsync(_path + "/" + "1");

            Assert.True(result.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ShouldDeleteUserAsync()
        {
            var username = "calvin";

            var result = await _client.PostAsync(_path,
                ContentHelper.GetStringContent(new UserContract {Username = username}));
            result.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
            var result2 = await _client.DeleteAsync(_path + "/" + user.Id);
            result2.EnsureSuccessStatusCode();
        }
    }
}