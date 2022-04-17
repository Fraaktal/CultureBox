namespace CultureBox.Model
{
    public enum RequestType { Borrow, Loan, All }
    public class LoanSearchRequest
    {
        /// <summary>
        /// Enumeration, can be: Borrow(0) the objets that you borrowed, Loan(1) the objects that you lent, All(2) all requests.
        /// </summary>
        public RequestType RequestType { get; set; }
        public string ApiKey { get; set; }
    }
}
