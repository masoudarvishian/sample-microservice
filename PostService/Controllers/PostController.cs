using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly Services.PostService _postService;

        public PostController(Services.PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
            try
            {
                return await _postService.GetPosts();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<Post> PostPost(Post post)
        {
            try
            {
                _postService.AddPost(post);
                return CreatedAtAction("GetPost", new { id = post.PostId }, post);
            }
            catch (System.Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
