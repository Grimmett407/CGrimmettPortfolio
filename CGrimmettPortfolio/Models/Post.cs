using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGrimmettPortfolio.Models
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string Media { get; set; }
        public string Slug { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}