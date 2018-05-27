using System.Collections.Generic;

namespace Forum.Models
{
    public class User
    {
        public User(int id, string userName, string password)
        {
            this.Id = id;
            this.UserName = userName;
            this.Password = password;
            this.PostIds = new List<int>();
        }

        public User(int id, string userName, string password, IEnumerable<int> postIds)
            : this (id, userName, password)
        {
            this.PostIds = new List<int>(postIds);
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public ICollection<int> PostIds { get; set; }
    } 
}

