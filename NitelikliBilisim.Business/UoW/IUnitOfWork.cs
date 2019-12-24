using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Business.UoW
{
    public interface IUnitOfWork
    {
        int Save();
    }
}
