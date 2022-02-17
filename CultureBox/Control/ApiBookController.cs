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

        public List<ApiBook> Search(APIRequestBook apiRequestBook)
        {
            return null;
        }
    }
}
