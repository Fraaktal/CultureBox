namespace CultureBox.Model
{
    public class LoanRequest
    {
        public int IdUser { get; set; }
        public int IdObject { get; set; }
        public string ApiKey { get; set; }

        /// <summary>
        /// Enumeration, can be: Book (0), Movie(1) or Series(2).
        /// </summary>
        public RequestObjectType RequestObjectType { get; set; }
    }
}
