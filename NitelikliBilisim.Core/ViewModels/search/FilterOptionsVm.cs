namespace NitelikliBilisim.Core.ViewModels.search
{
    public class FilterOptionsVm
    {
        public string[] categories { get; set; }
        public int[] ratings { get; set; }
    }

    public class SearchedEducationCategoryVm
    {
        public string name { get; set; }
        public int count { get; set; }
        public bool isChecked { get; set; } = false;
    }
}
