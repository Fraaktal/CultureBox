namespace CultureBox.Model
{
    public class ApiObjectToBorrow
    {
        public ApiObjectToBorrow(int objectId, int idUser)
        {
            IdOwner = idUser;
            IdObject = objectId;
        }

        public int IdOwner { get; set; }
        public int IdObject { get; set; }
    }
}
