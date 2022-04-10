using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CultureBox.Model
{
    public class ApiBookToBorrow
    {
        public ApiBookToBorrow(int bookId, int idUser)
        {
            IdOwner = idUser;
            IdBook = bookId;
        }

        public int IdOwner { get; set; }
        public int IdBook { get; set; }
    }
}
