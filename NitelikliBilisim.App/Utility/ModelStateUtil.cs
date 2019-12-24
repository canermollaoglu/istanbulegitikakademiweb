using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Utility
{
    public static class ModelStateUtil
    {
        public static IEnumerable<string> GetErrors(ModelStateDictionary modelStateDictionary)
        {
            foreach (var item in modelStateDictionary)
                foreach (var error in item.Value.Errors)
                    yield return error.ErrorMessage;
        }
    }
}
