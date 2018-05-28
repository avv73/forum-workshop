
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
	}
}
