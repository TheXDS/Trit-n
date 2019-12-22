using System;
using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.Models
{
    public class Comment : Model<long>
    {
        public User Author { get; set; }
        public Post Parent { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
    }
}