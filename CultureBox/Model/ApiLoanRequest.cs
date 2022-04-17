namespace CultureBox.Model
{
    public enum RequestState {Accepted, Refused, Pending, Ongoing, Ended}
    public enum RequestObjectType {Book, Movie, Series}

    public class ApiLoanRequest
    {
        public int Id { get; set; }
        public int IdRequester { get; set; }
        public int IdOwner { get; set; }
        public int IdObject { get; set; }

        /// <summary>
        /// Enumeration, can be: Accepted(0), Refused(1), Pending(2), Ongoing(3), Ended(4).
        /// </summary>
        public RequestState RequestState { get; set; }

        /// <summary>
        /// Enumeration, can be: Book (0), Movie(1) or Series(2).
        /// </summary>
        public RequestObjectType RequestType { get; set; }
    }
}
