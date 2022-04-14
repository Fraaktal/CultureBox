namespace CultureBox.Model
{
    public class LoanRequest
    {
        public int IdUser { get; set; }
        public int IdObject { get; set; }
        public string ApiKey { get; set; }
        public RequestObjectType RequestObjectType { get; set; }
    }
}
