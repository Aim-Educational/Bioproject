using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using CodeGenerator.Database;

namespace CodeGenerator.Common
{
    /// <summary>
    /// A class containing helper functions to ease working with the database.
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// [A very dirty-feeling function]
        /// Given a description, and a type, attempts to find an entry in the database for the given type that has a certain description.
        /// 
        /// For example, `getFromDescription<language>("C++")` would look for a language (in the language table) with the description of `C++` and then returns it.
        /// </summary>
        /// <typeparam name="T">The type to look for. This must be a database type.</typeparam>
        /// <param name="db">The database connection.</param>
        /// <param name="description">The description to look for.</param>
        /// <returns>Either null, if the value wasn't found, or the value that was found.</returns>
        public static T getFromDescription<T>(this DatabaseCon db, string description) where T : class
        {
            if(db == null)
                throw new ArgumentNullException("db");

            if(description == null)
                throw new ArgumentNullException("description");

            // Get the right set that represetns `T`
            var set = db._getSetFromT<T>();

            // Then look through all of the values in the DbSet for it.
            foreach(var value in set)
            {
                // This is dirty...
                dynamic dyValue = value;
                if(dyValue.description == description)
                    return value;
            }

            return null;
        }

        /// <summary>
        /// Gets a filtered set of error codes from the database.
        /// </summary>
        /// <param name="db">The database connection.</param>
        /// <param name="application">The application to filter by.</param>
        /// <param name="device">The device type to filter by.</param>
        /// <returns>All error codes that can be used by the chosen application and device type.</returns>
        public static IQueryable<error_code> getFilteredErrors(this DatabaseCon db, string application, string device)
        {
            var dbApp = db.getFromDescription<application>(application);
            var dbDevice = db.getFromDescription<device_type>(device);

            // These are variables since LINQ doesn't like it when I do it inside the query.
            var dbAppBit = (1 << dbApp.bit_index);
            var dbDevBit = (1 << dbDevice.bit_index);
            var query = from error in db.error_code
                        where ((error.application_ids & dbAppBit) > 0 || error.application_ids == 0)
                           && ((error.device_ids & dbDevBit) > 0 || error.device_ids == 0)
                        select error;

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="bitmask"></param>
        /// <returns></returns>
        public static List<T> getFromBitmask<T>(this DatabaseCon db, byte bitmask) where T : class
        {
            var set = db._getSetFromT<T>();

            // LINQ doesn't like using the _getBitIndex function during a query, so I go over it the old fashioned way.
            List<T> list = new List<T>();
            foreach(var value in set)
            {
                if((bitmask & (1 << value._getBitIndex())) > 0)
                    list.Add(value);
            }

            return list;
        }

        /// <summary>
        /// Throws an exception if the given bit index is already being used.
        /// 
        /// NOTE: The type `T` must contain fields called `bit_index` and `description`.
        /// </summary>
        /// <typeparam name="T">The type to check with. (this determines which table is used)</typeparam>
        /// <param name="db">The database connection</param>
        /// <param name="index">The index to check</param>
        public static void enforceBitIndexIsUnique<T>(this DatabaseCon db, byte index) where T : class
        {
            var set = db._getSetFromT<T>();

            foreach (var record in set)
            {
                dynamic dyRecord = record;
                if (dyRecord.bit_index == index)
                    throw new Exception($"The bit index {index} is being used by the {typeof(T).Name} '{dyRecord.description}'");
            }
        }

        /// <summary>
        /// Throws an exception if the given name/description (the field is called 'description' but is used more as a name) is already being used.
        /// 
        /// NOTE: The type `T` must contain a field called 'description'.
        /// </summary>
        /// <typeparam name="T">The type to check with. (this determines which table is used)</typeparam>
        /// <param name="db">The database connection.</param>
        /// <param name="name_description">The name/description to check for.</param>
        public static void enforceNameIsUnique<T>(this DatabaseCon db, string name_description) where T : class
        {
            var set = db._getSetFromT<T>();

            foreach (var record in set)
            {
                dynamic dyRecord = record;
                if (dyRecord.description == name_description)
                    throw new Exception($"The name/description '{name_description}' is already being used by another {typeof(T).Name}");
            }
        }

        // A hacky way to get around how generics work
        private static byte _getBitIndex(this object obj)
        {
            dynamic value = obj;
            return value.bit_index;
        }

        // Gets the right DbSet from the database, depending on what `T` is.
        private static DbSet<T> _getSetFromT<T>(this DatabaseCon db) where T : class
        {
            object set = null; // Contains the DbSet that we're gonna look use
            var type = typeof(T);

            // Depending on what the type of `T` is, get the corresponding DbSet from the database.
            if (type == typeof(language))
            {
                set = db.languages;
            }
            else if (type == typeof(application))
            {
                set = db.applications;
            }
            else if (type == typeof(device_type))
            {
                set = db.device_type;
            }
            else if(type == typeof(severity))
            {
                set = db.severities;
            }

            // Cast it, to make sure `T` is supported/we haven't messed up with what DbSet we're using.
            var castedSet = set as DbSet<T>;
            if (castedSet == null)
            {
                throw new ArgumentException($"Unsupported type: {type.ToString()}", "<T>");
            }

            return castedSet;
        }
    }
}
