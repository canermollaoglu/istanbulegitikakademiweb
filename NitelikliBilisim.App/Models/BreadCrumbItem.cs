namespace NitelikliBilisim.App.Models
{
    public class BreadCrumbItem
    {
        private readonly string _title;
        private string _url;
        private readonly string[] _routeParams;
        public BreadCrumbItem(string title, string url, params string[] routeParams)
        {
            _title = title;
            _url = url;
            _routeParams = routeParams;
        }

        public string Url
        {
            get
            {
                if (_routeParams.Length > 0)
                {
                    _url += "?";
                    for (int i = 0; i < _routeParams.Length; i++)
                        _url += i != _routeParams.Length - 1 ? $"{_routeParams[i]}&" : _routeParams[i];
                }

                return _url;
            }
        }
        public string Title => _title;
    }
}
