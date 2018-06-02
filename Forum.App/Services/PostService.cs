
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.App.Services
{
    using Forum.App.UserInterface.ViewModels;
    using Forum.Data;
    using Forum.Models;
    using System.Linq;

    public static class PostService
	{
        internal static Category GetCategory(int categoryId)
        {
            ForumData forumData = new ForumData();

            Category category = forumData.Categories.Find(c => c.Id == categoryId);

            return category;
        }

        internal static IList<ReplyViewModel> GetPostReplies(int postId)
        {
            ForumData forumData = new ForumData();

            Post post = forumData.Posts.Find(p => p.Id == postId);
            IList<ReplyViewModel> fetchedReplies = new List<ReplyViewModel>();

            foreach (int replyId in post.ReplyIds)
            {
                Reply currentReply = forumData.Replies.Find(r => r.Id == replyId);
                ReplyViewModel replyView = new ReplyViewModel(currentReply);
                fetchedReplies.Add(replyView);
            }

            return fetchedReplies;
        }

        internal static string[] GetAllCategoryNames()
        {
            ForumData forumData = new ForumData();

            string[] allCategories = forumData.Categories.Select(c => c.Name).ToArray();

            return allCategories;
        }

        internal static IEnumerable<Post> GetPostsByCategory(int categoryId)
        {
            ForumData forumData = new ForumData();
            // easier?
            IEnumerable<int> postIds = forumData.Categories.First(c => c.Id == categoryId).Posts;

            IEnumerable<Post> filteredPosts = forumData.Posts.Where(p => postIds.Contains(p.Id));
            return filteredPosts;
        }

        internal static PostViewModel GetPostViewModel(int postId)
        {
            ForumData forumData = new ForumData();

            Post post = forumData.Posts.Find(p => p.Id == postId);

            PostViewModel pvm = new PostViewModel(post);
            return pvm;
        }

        internal static bool TrySavePost(PostViewModel postView)
        {
            bool emptyCategory = string.IsNullOrWhiteSpace(postView.Category);
            bool emptyTitle = string.IsNullOrWhiteSpace(postView.Title);
            bool emptyContent = !postView.Content.Any();

            if (emptyCategory || emptyTitle || emptyContent)
            {
                return false;
            }

            ForumData forumData = new ForumData();

            Category category = EnsureCategory(postView, forumData);

            int postId = forumData.Posts.Any() ? forumData.Posts.Last().Id + 1 : 1;

            User author = UserService.GetUser(postView.Author, forumData);

            int authorId = author.Id;
            string content = string.Join("", postView.Content);

            Post post = new Post(postId, postView.Title, content, category.Id, author.Id);

            forumData.Posts.Add(post);
            author.Posts.Add(postId);
            category.Posts.Add(postId);
            forumData.SaveChanges();

            postView.PostId = postId;
            return true;
        }

        internal static bool TrySaveReply(ReplyViewModel replyView, int postId)
        {
            if (!replyView.Content.Any())
            {
                return false;
            }

            ForumData forumData = new ForumData();

            User author = UserService.GetUser(replyView.Author, forumData);
            Post post = forumData.Posts.Find(p => p.Id == postId);

            int authorId = author.Id;
            string content = string.Join("", replyView.Content);

            int replyId = forumData.Replies.Any() ? forumData.Replies.Last().Id + 1 : 1;

            Reply reply = new Reply(replyId, content, authorId, postId);
            
            forumData.Replies.Add(reply);
            post.ReplyIds.Add(replyId);
            forumData.SaveChanges();
            return true;
        }

        private static Category EnsureCategory(PostViewModel postView, ForumData forumData)
        {
            string categoryName = postView.Category;
            Category category = forumData.Categories.FirstOrDefault(x => x.Name == categoryName);
            if (category == null)
            {
                List<Category> categories = forumData.Categories;
                int categoryId = categories.Any() ? categories.Last().Id + 1 : 1;
                category = new Category(categoryId, categoryName, new List<int>());
                forumData.Categories.Add(category);
            }

            return category;
        }
	}
}
