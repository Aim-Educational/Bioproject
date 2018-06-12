using System;
using System.Windows.Forms;

${
    // At some point in the future, the generator might want to get the namespace
    // and class name automatically, instead of it being hard coded into the template.
}
namespace DataUserInterface.Forms
{
    public partial class SearchForm : Form
    {
        void openEditorByType(EnumSearchFormType type, EnumEditorMode mode, int id = -1)
        {
            Form form;
            switch(this.type)
            {
                ${custom_EditorCaseStatements}

                default:
                    throw new NotImplementedException($"No editor for type: {this.type}");
            }
        }
    }
}

${
    // Variables Avaliable:
    //      Model model = The Model that's being used.
    //      string custom_EditorCaseStatements = Case statements for every object in the model.
    //                                           The code for this is generated from 'searchFromEditorCases.cs'
}