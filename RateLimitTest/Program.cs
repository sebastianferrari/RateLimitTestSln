using MessageLogger.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RateLimitTest
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main()
        {
            Debug.Print("============ Process Started =============");
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var _continue = true;
                var _counter = 0;

                while (_continue)
                {
                    var display_name = $"TestApplicationName{_counter}";

                    await RegisterApplicationAsync(display_name);

                    _counter++;

                    if (_counter > 200) _continue = false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception ---> {e.Message}");
            }

            Console.WriteLine("Press any key to terminate.");
            Console.ReadLine();
        }

        static async Task<Uri> RegisterApplicationAsync(string display_name)
        {
            ApplicationDTO app = new ApplicationDTO
            {
                display_name = display_name
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("register", app);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Status code in client side: ", response.StatusCode);
            }

            response.EnsureSuccessStatusCode();

            Debug.Print("Response Status Code: ---> {0}", response.StatusCode.ToString());

            // Return the URI of the created resource.
            return response.Headers.Location;
        }
    }
}