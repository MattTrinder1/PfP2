namespace Common.Models.Business
{
    public class User : EntityBase
    {
        public User() { }

        public string BadgeNumber { get; set; }

        public string EmailAddress { get; set; }

        public string DisplayValue { get; set; }
    }
}
