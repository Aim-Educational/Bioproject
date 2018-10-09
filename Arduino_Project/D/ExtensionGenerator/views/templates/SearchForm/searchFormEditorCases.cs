${
    // Note that this is not a whole file, but a template that generates a portion of the code
    // inside of 'searchFormMain.cs'

    // While it looks annoying in this single file, the white space is important as it should
    // line up properly with the main template file.
}
                case EnumSearchFormType.${custom_FixedObjectName}:
                    form = new Form${custom_FixedObjectName}Editor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;
${
    // Variables Avaliable:
    //      [Everything in the main template file]
    //      string custom_FixedObjectName = A version of the object's name that is 'fixed' to follow a standard
    //                                      way of naming. e.g. 'device_type' -> 'DeviceType'
    //
    // Notes:
    //  Because of the format of 'custom_FixedObjectName', everything must follow it's naming format.
}