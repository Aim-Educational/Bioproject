using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Maintainer.Controls;

namespace Maintainer.Interfaces
{
    public interface ISearchProvider
    {
        void populateDataGrid(DataGrid grid);
    }
}
