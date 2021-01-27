using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hollan.Function
{
    public class HttpTrigger
    {
        private readonly BloggingContext _context;
        public HttpTrigger(BloggingContext context)
        {
            _context = context;
        }

        [FunctionName("GetPosts")]
        public HttpResponseData GetPosts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "posts")] HttpRequestData req,
            ILogger log)
        {
            log.LogInformation("C# HTTP GET/posts trigger function processed a request.");

            var postsArray = _context.Posts.OrderBy(p => p.Title).ToArray();
            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(postsArray));
        }

        [FunctionName("PostBlogSite")]
        public async Task<HttpResponseData> CreateBlogAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blog")] HttpRequestData req,
            CancellationToken cts,
            ILogger log)
        {
            log.LogInformation("C# HTTP POST/blog trigger function processed a request.");

            var entity = await _context.Blogs.AddAsync(new Blog(), cts);
            await _context.SaveChangesAsync(cts);
            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(entity.Entity));
        }
    }
}
