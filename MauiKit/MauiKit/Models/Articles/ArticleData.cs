﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiKit.Models
{
    public class ArticleData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Section { get; set; }
        public Color SectionColor { get; set; }
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string BackgroundImage { get; set; }
        public string VideoUrl { get; set; }
        public string Quote { get; set; }
        public string QuoteAuthor { get; set; }
        public string When { get; set; }
        public string Followers { get; set; }

        public List<ArticleCommentData> Comments { get; set; }
        public string Likes { get; set; }
    }

    public class ArticleCommentData
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsStared { get; set; }
        public bool IsRead { get; set; }
        public int ThreadCount { get; set; }
        public string When { get; set; }
        public ArticleCommentOwnerData From { get; set; }
    }

    public class ArticleCommentOwnerData
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public class ArticleCardData
    {
        public string Icon { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
