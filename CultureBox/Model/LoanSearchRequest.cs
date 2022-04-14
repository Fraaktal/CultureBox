namespace CultureBox.Model
{
    //Borrow => objet que j'ai emprunté à quelqu'un, Loan => Objet que j'ai prêté à quelqu'un.
    public enum RequestType { Borrow, Loan, All }
    public class LoanSearchRequest
    {
        public RequestType RequestType { get; set; }
        public string ApiKey { get; set; }
    }
}
