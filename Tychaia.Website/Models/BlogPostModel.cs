// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Tychaia.Website.Models
{
    public class BlogPostModel
    {
        public int ID { get; set; }
        public int? Previous { get; set; }
        public int? Next { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public long UNIXDatePublished { get; set; }
        public string Uri { get; set; }
    }
}
