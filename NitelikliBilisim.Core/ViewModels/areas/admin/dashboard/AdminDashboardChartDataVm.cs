namespace NitelikliBilisim.Core.ViewModels.areas.admin.dashboard
{
    public class AdminDashboardChartDataVm
    {
        public ApexChartModel[] Values { get; set; }
    }
    public class ApexChartModel
    {
        public string x { get; set; }
        public string y { get; set; }
    }
}
