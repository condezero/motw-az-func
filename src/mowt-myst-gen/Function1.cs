using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Stubble.Core.Builders;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace mowt_myst_gen
{
    public static class Function1
    {


        [FunctionName("Generator")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {



            log.LogInformation("C# HTTP trigger function processed a request.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

          
            string html = await GetHtml(log);
            if (string.IsNullOrEmpty(html))
            {
                log.LogError("Cannot find the html file");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
              

            }
            var stubble = new StubbleBuilder().Build();
            var output = await stubble.RenderAsync(html, new { loc = Data.GetLocation(), mon = Data.GetMonster(), typ = Data.GetMonsterType(), mot = Data.GetMotivation() });
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(output);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private static async Task<string> GetHtml(ILogger log)
        {
            var fileNs = typeof(Function1).Namespace;
            log.LogInformation($"Namespace: {fileNs}");
            using var stream = typeof(Function1).Assembly.GetManifestResourceStream(fileNs + ".index.html");
            if (stream is null)
            {
                return string.Empty;
            }
            using var sr = new StreamReader(stream);

            var html = await sr.ReadToEndAsync();

            return html;
        }
        private static string GetMessage() => $"En el/la {Data.GetLocation()} se encuentra {Data.GetMonster()}, con el aspecto {Data.GetMonsterType()} motivada[o] {Data.GetMotivation()}";

    }

}
