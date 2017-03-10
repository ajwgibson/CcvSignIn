using CcvSignIn.Properties;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CcvSignIn.Service
{
    class ApiService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ApiService));

        private static string token = null;

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Settings.Default.ApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (token != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public static async Task Login(string username, string password)
        {
            var client = CreateClient();

            var login = new LoginData { email = username, password = password };
            HttpResponseMessage response = await client.PostAsJsonAsync("auth_user", login);

            if (response.IsSuccessStatusCode)
            {
                logger.DebugFormat("Login succeeded for username {0}", username);

                var loginResponse = await response.Content.ReadAsAsync<LoginResponse>();
                token = loginResponse.auth_token;
            }
            else
            {
                logger.DebugFormat("Login failed for username {0}", username);
            }
        }

        public static async Task<List<CcvSignIn.Model.Child>> DownloadChildren()
        {
            var client = CreateClient();

            List<CcvSignIn.Model.Child> results = new List<Model.Child>();

            HttpResponseMessage response = await client.GetAsync("v1/children");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                List<Child> children = JsonConvert.DeserializeObject<List<Child>>(jsonString);

                //foreach (var child in children)
                //{
                //    Console.WriteLine(child);

                //    results.Add(
                //        new Model.Child
                //        {
                //            Id = child.id,
                //            First = child.first_name,
                //            Last = child.last_name,
                //        });
                //}

                logger.InfoFormat("v1/children api call returned {0} children", children.Count);

                var query = from c in children
                            select new Model.Child
                            {
                                Id = c.id,
                                First = c.first_name,
                                Last = c.last_name,
                            };

                return query.ToList();
            }
            else
            {
                logger.Debug("v1/children api call failed");
            }

            return results;
        }


        class LoginData
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        class LoginResponse
        {
            public string auth_token { get; set; }
        }

        class Child
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string date_of_birth { get; set; }
        }
    }

}
