using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CultureBox.APIControllers;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ILoanRequestControllerDAO
    {
        List<ApiLoanRequest> GetAllRequests(RequestType requestType, int id);
        void RequestLoan(int brIdBook, int brIdUser, int idBorrower, RequestObjectType type);
        ApiLoanRequest GetRequestById(int id);
        bool UpdateLoanRequest(ApiLoanRequest lr);
        bool IsBorrowed(int brIdBook, int brIdUser, RequestObjectType type);
    }

    public class LoanRequestControllerDAO : ILoanRequestControllerDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public LoanRequestControllerDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        
        public List<ApiLoanRequest> GetAllRequests(RequestType requestType, int id)
        {
            List<ApiLoanRequest> res = null;
            
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiLoanRequest>("apiloanrequest");

                switch (requestType)
                {
                    case RequestType.All:
                        res = col.Find(x => x.IdOwner == id || x.IdRequester == id).ToList();
                        break;
                    case RequestType.Borrow:
                        res = col.Find(x => x.IdRequester == id).ToList();
                        break;
                    case RequestType.Loan:
                        res = col.Find(x => x.IdOwner == id).ToList();
                        break;
                }
            });

            return res;
        }

        public void RequestLoan(int idBook, int idOwner, int idBorrower, RequestObjectType type)
        {
            ApiLoanRequest r = new ApiLoanRequest()
            {
                IdBook = idBook,
                IdOwner = idOwner,
                IdRequester = idBorrower,
                RequestState = RequestState.Pending,
                RequestType = type
            };

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiLoanRequest>("apiloanrequest");
                col.Insert(r);
            });
        }

        public ApiLoanRequest GetRequestById(int id)
        {
            ApiLoanRequest res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiLoanRequest>("apiloanrequest");

                res = col.FindOne(x => x.Id == id);
            });

            return res;
        }

        public bool UpdateLoanRequest(ApiLoanRequest lr)
        {
            bool res = false;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiLoanRequest>("apiloanrequest");

                res = col.Update(lr);
            });

            return res;
        }

        public bool IsBorrowed(int IdBook, int IdUser, RequestObjectType type)
        {
            bool res = false;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiLoanRequest>("apiloanrequest");

                var result = col.FindOne(x => x.IdOwner == IdUser && 
                                              x.IdRequester == IdBook &&
                                              x.RequestState != RequestState.Ended
                                              && x.RequestType == type);
                res = result != null;
            });

            return res;
        }
    }
}
