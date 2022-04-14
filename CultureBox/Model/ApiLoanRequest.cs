﻿namespace CultureBox.Model
{
    public enum RequestState {Accepted, Refused, Pending, Ongoing, Ended}
    public enum RequestObjectType {Book, Movie, Series}

    public class ApiLoanRequest
    {
        public int Id { get; set; }
        public int IdRequester { get; set; }
        public int IdOwner { get; set; }
        public int IdObject { get; set; }
        public RequestState RequestState { get; set; }
        public RequestObjectType RequestType { get; set; }
    }
}
