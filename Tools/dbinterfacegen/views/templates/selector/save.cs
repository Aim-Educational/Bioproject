$PLACEHOLDERS
    $NAME
    $OBJECT_TYPE
    $OBJECT_TABLE
    $OBJECT_PK
$END
$FINISH_CONFIG
data.$NAME = new Func<$OBJECT_TYPE>(() => { foreach(var v in db.$OBJECT_TABLE){ if(v.$OBJECT_PK == (this.$NAME.item as $OBJECT_TYPE).$OBJECT_PK) return v; } return null; })();
