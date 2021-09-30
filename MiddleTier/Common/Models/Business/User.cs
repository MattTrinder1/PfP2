namespace Common.Models.Business
{
    public class User : EntityBase
    {
        public User() { }

        public string cp_badgenumber { get; set; }

        public string internalemailaddress { get; set; }

        public string fullname { get; set; }

        public string firstname { get; set; }


        public string DisplayValue { get; set; }
    }
}
