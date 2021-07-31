using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Services
{
    public class PostService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly PostServiceContext _dbContext;

        public PostService(UnitOfWork unitOfWork, PostServiceContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<Post>> GetPosts()
        {
            var all = await _unitOfWork.Repository<Post>().FindAll().Include(x => x.User)?.ToListAsync();
            return all;
        }

        public void AddPost(Post post)
        {
            // we can use entity framework to add a post
            //_unitOfWork.Repository<Post>().Create(post);
            //await _unitOfWork.SaveChangesAsync();

            // or we can call a stored procudure
            var command = $"EXEC spAddPost '{post.Title}', '{post.Content}', {post.UserId}";

            _dbContext.Database.ExecuteSqlRaw(command);
        }
    }
}
