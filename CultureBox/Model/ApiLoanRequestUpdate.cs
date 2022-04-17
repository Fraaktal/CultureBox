namespace CultureBox.Model
{
    public class ApiLoanRequestUpdate
    {
        /// <summary>
        /// Enumeration, can be: Accepted(0), Refused(1), Pending(2), Ongoing(3), Ended(4).
        /// </summary>
        public RequestState RequestState { get; set; }
        public string ApiKey { get; set; }
    }
}
