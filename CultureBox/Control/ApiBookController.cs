using System.Collections.Generic;
using CultureBox.APIControllers;
using CultureBox.Model;

namespace CultureBox.Control
{
    public interface IApiBookController
    {
        List<ApiBook> Search(APIRequestBook apiRequestBook);
    }

    public class ApiBookController : IApiBookController
    {
        private const string APIKEY = "key=AIzaSyAwFEt2OT_741yiGFP5GSW3CULc4W5KiEs";

        public List<ApiBook> Search(APIRequestBook apiRequestBook)
        {
            return null;
        }
    }
}
