using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace BambooToothpicks {
    // FeedData
    // Holds info for a single blog feed, including a list of blog posts (FeedItem)
    public class FeedData {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public string Description { get; set; }

        public List<FeedItem> Items { get; set; }
    }

    // FeedItem
    // Holds info for a single blog post
    public class FeedItem {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
        public string PubDate { get; set; }
        public string Description { get; set; }
    }
}