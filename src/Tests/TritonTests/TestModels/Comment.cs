using System;
using TheXDS.Triton.Models.Base;

#nullable enable

namespace TheXDS.Triton.TestModels
{
    internal class Comment : Model<long>
    {
        public User Author { get; set; }
        public Post Parent { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
    }
}