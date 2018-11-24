$PLACEHOLDERS
    $NAME
    $DATA_TYPE
$END
$CONDITIONAL_PLACEHOLDERS
    $DATA_TYPE=string  $> $CONVERT=/**/
    $DATA_TYPE=int     $> $CONVERT=Convert.ToInt32
    $DATA_TYPE=long    $> $CONVERT=Convert.ToInt64
    $DATA_TYPE=float   $> $CONVERT=Convert.ToSingle
    $DATA_TYPE=double  $> $CONVERT=Convert.ToDouble
    $DATA_TYPE=decimal $> $CONVERT=Convert.ToDecimal
$END
$FINISH_CONFIG
data.$NAME = $CONVERT(this.$NAME.Text);
