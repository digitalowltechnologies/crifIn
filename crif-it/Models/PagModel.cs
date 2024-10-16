namespace Crif.It.Models
{
    public class PagModel
    {
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public string RequestParameter { get; set; } = "page";
    }
}