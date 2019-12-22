using System;
using System.Collections.Generic;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models
{
    public class User : Model<string>
    {
        public string PublicName { get; set; } = null!;
        public DateTime Joined { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}