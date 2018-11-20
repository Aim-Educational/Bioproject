$PLACEHOLDERS
    $HEADER     The header title.
    $VARIABLE   The variable to bind to.
    $TABLE_NAME The name of the table object.
$END
$FINISH_CONFIG
grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "$HEADER",
                    Binding = new Binding(nameof($TABLE_NAME.$VARIABLE))
                });
                