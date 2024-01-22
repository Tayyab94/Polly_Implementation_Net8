using Microsoft.AspNetCore.Mvc;
using Polly;
using RestSharp;
using static System.Net.WebRequestMethods;

namespace Robust_resileint_Sample.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServiceController : ControllerBase
    {
        
        private readonly ILogger<WeatherForecastController> _logger;

        public ServiceController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<IActionResult>Get()
        //{
        //    var url = "https://moviesdatabase.p.rapidapi.com/titles/utils/titleTypes";
        //    var client = new RestClient();
        //    var request = new RestRequest(url,Method.Get);
        //    request.AddHeader("X-RapidAPI-Key", "3c778ae75fmshbb7a62d83e59d7cp1ced67jsn57baa016c80c");
        //    request.AddHeader("X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com");
        //    var response =await client.ExecuteAsync(request);

        //    if(response.IsSuccessful)
        //    {
        //        return Ok(response.Content);
        //    }

        //    return BadRequest(response.ErrorMessage);
        //}

        [HttpGet(Name ="Call_RestAPI")]
        public async Task<IActionResult> Call_RestAPI()
        {
            var amountOftime = TimeSpan.FromSeconds(6);

            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 4, sleepDurationProvider: a => amountOftime, onRetry: (exception, retyrCount) =>
                {
                    Console.WriteLine($"Error: {exception.Message} - Retry : {retyrCount}");
                });

            string response = await retryPolicy.ExecuteAsync(async () =>
            {
                var url = "https://moviesdatabase.p.rapidapi.com/titles/utils/titleTypes";
                var client = new RestClient();
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("X-RapidAPI-Key", "3c778ae75fmshbb7a62d83e59d7cp1ced67jsn57baa016c80c");
                request.AddHeader("X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com");

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                    return response.Content;

                throw new Exception($"{response.Content}");
            });

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult>GET()
        {
            var amountOfTime = TimeSpan.FromSeconds(6);

            // This line of code, Retry to Execute instantely..  below one, we have added the timespan for 6 seconds which means that now the control will wait for 6 sec then it will reExecute the code
            //var retryPolicy = Policy
            //    .Handle<Exception>()
            //    .RetryAsync(retryCount: 5, onRetry: (exception, retryCount) =>
            //    {
            //        Console.WriteLine($"Error: {exception.Message} - Retry : {retryCount}");
            //    });

            var retryPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(retryCount: 5,sleepDurationProvider: i=> amountOfTime, onRetry: (exception, retryCount) =>
               {
                   Console.WriteLine($"Error: {exception.Message} - Retry : {retryCount}");
               });


            var response =await retryPolicy.ExecuteAsync(async () => await ConnectionToMoviesAPI());

            return Ok(response);
        }

        private async Task<string> ConnectionToMoviesAPI()
        {
            var url = "https://moviesdatabase.p.rapidapi.com/titles/utils/titleTypes";
            var client = new RestClient();
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("X-RapidAPI-Key", "3c778ae75fmshbb7a62d83e59d7cp1ced67jsn57baa016c80c");
            request.AddHeader("X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com");
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return response?.Content;

            throw new Exception($"{response.Content}");
           
        }
    }
}
