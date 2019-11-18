namespace NitelikliBilisim.App.Models
{
    public class BreadCrumbItem
    {
        private readonly string _title;
        private readonly string _url;
        public BreadCrumbItem(string title, string url)
        {
            _title = title;
            _url = url;
        }

        public string Url => _url;
        public string Title => _title;
    }
}
