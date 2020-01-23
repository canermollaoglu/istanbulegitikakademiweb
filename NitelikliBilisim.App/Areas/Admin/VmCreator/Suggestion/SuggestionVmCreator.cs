using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.Suggestion
{
    public class SuggestionVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public SuggestionVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


    }
}
