using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
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
            FunctionExecutionContext context)
        {
            context.Logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");

           var postsArray = _context.Posts.OrderBy(p => p.Title).ToArray();
           return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(postsArray));
        }

        [FunctionName("CreateBlog")]
        public async Task<HttpResponseData> CreateBlogAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blog")] HttpRequestData req,
            FunctionExecutionContext context)
        {
            context.Logger.LogInformation("C# HTTP POST/blog trigger function processed a request.");

            var blog = JsonConvert.DeserializeObject<Blog>(Encoding.UTF8.GetString(req.Body.Value.ToArray()));
            context.Logger.LogInformation(JsonConvert.SerializeObject(blog));

            var entity = await _context.Blogs.AddAsync(blog, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(entity.Entity));
        }

        [FunctionName("CreatePost")]
        public async Task<HttpResponseData> CreatePostAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "post")] HttpRequestData req,
            FunctionExecutionContext context)
        {
            context.Logger.LogInformation("C# HTTP POST/blog trigger function processed a request.");

            var post = JsonConvert.DeserializeObject<Post>(Encoding.UTF8.GetString(req.Body.Value.ToArray()));
            var entity = await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync(CancellationToken.None);
            return new HttpResponseData(HttpStatusCode.OK, JsonConvert.SerializeObject(entity.Entity));
        }
    }
}
