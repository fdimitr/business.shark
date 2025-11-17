namespace BusinessSharkClient.Logic.Models
{
    public partial class SawmillListGroup : List<SawmillListModel>
    {
        public string City { get; set; }
        public string CountryFlag { get; set; }
        public SawmillListGroup(string city, string countryFlag, IEnumerable<SawmillListModel> sawmills) : base(sawmills)
        {
            City = city;
            CountryFlag = countryFlag;
        }
    }
}
