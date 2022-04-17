namespace CultureBox.Model
{
    public class SearchObjectToBorrowRequest
    {
        /// <summary>
        /// Enumeration, can be: Book (0), Movie(1) or Series(2).
        /// </summary>
        public RequestObjectType RequestObjectType { get; set; }
        public string Title { get; set; }
    }
}
