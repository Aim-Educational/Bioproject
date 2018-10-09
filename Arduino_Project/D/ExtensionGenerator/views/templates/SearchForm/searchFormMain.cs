using System;
using System.Linq;
using System.Windows.Forms;
using DataManager.Model;
${
    // At some point in the future, the generator might want to get the namespace
    // and class name automatically, instead of it being hard coded into the template.
}
namespace DataUserInterface.Forms
{
    public enum EnumSearchFormType
    {
        ${custom_EnumValues}
    }

    public partial class SearchForm : Form
    {
        void populateSearchResultsForType(EnumSearchFormType type)
        {
            using (var db = new PlanningContext())
            {
                switch(type)
                {
                    ${custom_DataCaseStatements}

                    default:
                        throw new NotImplementedException($"No handler for type: {type}");
                }
            }
        }

        void openEditorByType(EnumSearchFormType type, EnumEditorMode mode, int id = -1)
        {
            Form form;
            switch(type)
            {
                ${custom_EditorCaseStatements}

                default:
                    throw new NotImplementedException($"No editor for type: {type}");
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