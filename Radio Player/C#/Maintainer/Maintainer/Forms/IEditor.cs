using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintainer.Forms
{
    public interface IEditor
    {
        EditorType type { get; set; }
        int objectID    { get; set; }

        void onUpdateUI();

        void onCreate();
        void onUpdate();
        void onDelete();
    }
}
