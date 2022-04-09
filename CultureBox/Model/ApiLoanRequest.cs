namespace CultureBox.Model
{
    public enum RequestState {Accepted, Refused, Pending, Ongoing, Ended}

    public class ApiLoanRequest
    {
        public int Id { get; set; }
        public int IdRequester { get; set; }
        public int IdOwner { get; set; }
        public int IdBook { get; set; }
        public RequestState RequestState { get; set; }
    }
}
