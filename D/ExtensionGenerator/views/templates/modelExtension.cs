/// Generated by ExtensionGenerator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ${model.namespace}
{
    public partial class ${object.className} : IDataModel
    {
        public bool isOutOfDate(${model.context.className} db)
        {
            var obj = db.${dbSet.variableName}.SingleOrDefault(d => d.${object.keyName} == this.${object.keyName});

            var dbTimestamp = BitConverter.ToInt64(obj.timestamp, 0);
            var localTimestamp = BitConverter.ToInt64(this.timestamp, 0);

            return (dbTimestamp > localTimestamp);
        }

        public bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no)
        {
            using (var db = new ${model.context.className}())
            {
                var obj = db.${dbSet.variableName}.SingleOrDefault(d => d.${object.keyName} == this.${object.keyName});
                
                if (this.isOutOfDate(db) && obj.version <= this.version)
                {
                    if(shouldIncrement == IncrementVersion.yes)
                        this.version += 1;

                    return true;
                }

                return false;
            }
        }
    }
}

${// [Generation Notes]
  // The contents of this file are passed to a 'mixin(scriptlike.interp!"")' statement,
  // so the format for running D code inside of the 'interp' function can be used to generate
  // most of the needed code.
  //
  // For example, this comment is inside a code execution block, so won't actually show up
  // in the final generated file.
  //
  // Variables Useable:
  //      TableObject object = The TableObject this extension is being made for.
  //      Model       model  = The Model being used.
  //      DbSet       dbSet  = The DbSet inside of the model's DbContext class which contains objects that
  //                           this extension is being generated for. 'E.G DbSet<device>'
}