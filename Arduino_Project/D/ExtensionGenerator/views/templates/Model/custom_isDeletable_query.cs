            var ${custom_isDeletable_queryName} = from obj in db.${custom_isDeletable_queryDependantSet.variableName}
                         where obj.${dependant.dependantFK.variableName} == this.${object.keyName}
                         select obj;