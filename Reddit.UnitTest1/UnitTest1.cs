using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Reddit.Models;
using Reddit.Repositories;

namespace Reddit.UnitTest1
{
    public class UnitTest1
    {
        //Since we need to test PagedList and it's CreateAsync method, we'll do it through PostRepository method GetPost, because the method it returns: PagedList<Post>.CreateAsync(posts, page, pageSize); 
        private IPostsRepository GetPostsRepostory()
        {

            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            dbContext.Posts.Add(new Post { Title = "Title 1", Content = "Content 1", Upvotes = 5, Downvotes = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 2", Content = "Content 1", Upvotes = 12, Downvotes = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 3", Content = "Content 1", Upvotes = 3, Downvotes = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 4", Content = "Content 1", Upvotes = 221, Downvotes = 1 }); 
            dbContext.Posts.Add(new Post { Title = "Title 5", Content = "Content 1", Upvotes = 5, Downvotes = 2123 });
            dbContext.Posts.Add(new Post { Title = "Title 6", Content = "Content 1", Upvotes = 70, Downvotes = 23 });
            dbContext.SaveChanges();

            return new PostsRepository(dbContext);
        }

        private IPostsRepository GetEmptyPostsRepostory()
        {

            string dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            dbContext.SaveChanges();

            return new PostsRepository(dbContext);
        }


        [Fact]
        public async Task PagedList_ReturnsCorrectPaging1()
        {
            var postsRepository = GetPostsRepostory();
            var posts = await postsRepository.GetPosts(1, 2, null, null, null);

            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
        }


        [Fact]
        public async Task PagedList_ReturnsCorrectPaging2()
        {
            var postsRepository = GetPostsRepostory();
            var posts = await postsRepository.GetPosts(2, 2, null, null, null);

            Assert.True(posts.HasNextPage);
            Assert.True(posts.HasPreviousPage);

        }
        [Fact]
        public async Task PagedList_ReturnsCorrectPaging3()
        {
            var postsRepository = GetPostsRepostory();
            var posts = await postsRepository.GetPosts(2, 3, null, null, null);

            Assert.False(posts.HasNextPage);
            Assert.True(posts.HasPreviousPage);

        }
        
        [Fact]
        public async Task PagedList_InvalidPageZero_ThrowsArgumentException()
        {
            var postsRepository = GetPostsRepostory();

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => postsRepository.GetPosts(0, 10, null, null, null));
            Assert.Equal("page", exception.ParamName);
        }
   
        [Fact]
        public async Task PagedList_InvalidPageSize_ThrowsArgumentOutOfRangeException()
        {
            var postsRepository = GetPostsRepostory();

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => postsRepository.GetPosts(1, 0, null, null, null));
            Assert.Equal("pageSize", exception.ParamName);
        }

        [Fact]
        public async Task PagedList_TotalCountIsMoreThanSize()
        {
            var postsRepository = GetPostsRepostory();
            var posts = await postsRepository.GetPosts(1, 2, null, null, null);

            Assert.Equal(2, posts.Items.Count);
            Assert.Equal(6, posts.TotalCount);
        }

        [Fact]
        public async Task PagedList_EmptyList()
        {
            var postsRepository = GetEmptyPostsRepostory();
          

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => postsRepository.GetPosts(0, 10, null, null, null));
            Assert.Equal("page", exception.ParamName);
        }
            
    }
}