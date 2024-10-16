using Examine;

namespace Crif.It.Models
{
    public partial class SearchResults
    {
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<ISearchResult> Results { get; set; }
    }
}
