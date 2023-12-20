// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RebacExperiments.Server.Api.Tests
{
    public class JsonSerializerTests : TransactionalTestBase
    {
        private class Person
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public async Task ListUserObjects_OneUserTaskAssignedThroughOrganizationAndTeam()
        {
            // Arrange
            var json = @"
            {
                ""@odata.context"": ""https://graph.microsoft.com/v1.0/$metadata#users('48d31887-5fad-4d73-a9f5-3c356e68a038')/contacts"",
                ""@odata.count"": 34,
                ""value"": [
                    {
                        ""id"": 1,
                        ""name"": ""Philipp Wagner""
                    },
                    {
                        ""id"": 2,
                        ""name"": ""Max Powers""
                    }
                ],
                ""@odata.nextLink"": ""https://graph.microsoft.com/v1.0/me/contacts?%24count=true&%24skip=10""
            }";


            // Act

            dynamic a = JsonNode.Parse(json);

            var jsonObject = JsonSerializer.Deserialize<JsonObject>(json);
            var das = (object)jsonObject["@odata.count"];
            // Assert
            var metadata = jsonObject
                .Where(x => x.Key.StartsWith("@odata"))
                .Select(x => (object?) jsonObject[x.Key])                
                .ToList();
            
            var aa = jsonObject["value"].Deserialize<List<Person>>();
        }

    }
}