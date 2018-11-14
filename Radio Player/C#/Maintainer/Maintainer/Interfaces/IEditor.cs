using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintainer.Interfaces
{
    public interface IEditor : ISearchAction
    {
        void flagAsCreateMode();
        void saveChanges();
        bool isDataDirty { get; }
    }
}
