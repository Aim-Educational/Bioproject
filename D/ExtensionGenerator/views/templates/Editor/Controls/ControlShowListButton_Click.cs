private void ${this.name}_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.${this.objectField.typeName.standardisedName});
    form.MdiParent = this.MdiParent;
    form.Show();
}
