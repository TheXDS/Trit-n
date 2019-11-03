﻿using System;
using System.Collections.Generic;
using TheXDS.Triton.Models.Base;

#nullable enable

namespace TheXDS.Triton.TestModels
{
    internal class Post : Model<long>
    {
        public string Title { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public bool Published { get; set; }
        public string Content { get; set; } = null!;
        public User Author { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}