﻿namespace Forum.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Forum.App.Controllers.Contracts;
    using Forum.App.Services;
    using Forum.App.UserInterface.Contracts;
    using Forum.App.Views;
    using Forum.Models;

    public class CategoryController : IController, IPaginationController
    {
        public const int PAGE_OFFSET = 10;
        private const int COMMAND_COUNT = PAGE_OFFSET + 3;

        public CategoryController()
        {
            this.CurrentPage = 0;
            this.SetCategory(0);    
        }

        private enum Command
        {
            Back,
            ViewPost,
            PreviousPage,
            NextPage
        }

        public int CurrentPage { get; set; }

        public int CategoryId { get; set; }

        private string[] PostTitles { get; set; }

        private int LastPage => this.PostTitles.Length / 11;

        private bool IsFirstPage => this.CurrentPage == 0;

        private bool IsLastPage => this.CurrentPage == this.LastPage;

        public MenuState ExecuteCommand(int index)
        {
            switch ((Command)index)
            {
                case Command.Back:

                    return MenuState.Back;
                case Command.ViewPost:

                    return MenuState.ViewPost;
                case Command.PreviousPage:

                    return MenuState.ViewPost;
                case Command.NextPage:

                    return MenuState.ViewPost;
            }

            throw new InvalidCommandException();
        }

        public IView GetView(string userName)
        {
            GetPosts();
            string categoryName = PostService.GetCategory(this.CategoryId).Name;
            return new CategoryView(categoryName, this.PostTitles, this.IsFirstPage, this.IsLastPage);
        }

        public void SetCategory(int categoryId)
        {
            this.CategoryId = categoryId;
        }

        private void ChangePage(bool forward = true)
        {
            this.CurrentPage += forward ? 1 : -1;
            GetPosts();
        }

        private void GetPosts()
        {
            IEnumerable<Post> allCategoryPosts = PostService.GetPostsByCategory(this.CategoryId);

            this.PostTitles = allCategoryPosts
                .Skip(this.CurrentPage * PAGE_OFFSET)
                .Take(PAGE_OFFSET)
                .Select(p => p.Title)
                .ToArray();
        }
    }
}
