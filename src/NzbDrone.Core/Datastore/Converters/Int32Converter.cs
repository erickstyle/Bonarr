using System;
using Marr.Data.Converters;
using Marr.Data.Mapping;

namespace NzbDrone.Core.Datastore.Converters
{
    public class Int32Converter : IConverter
    {
        public object FromDB(ConverterContext context)
        {
            if (context.DbValue == DBNull.Value)
            {
                return DBNull.Value;
            }

            if (context.DbValue is int)
            {
                return context.DbValue;
            }

            return Convert.ToInt32(context.DbValue);
        }

        public object FromDB(ColumnMap map, object dbValue)
        {
            if (dbValue == DBNull.Value)
            {
                return DBNull.Value;
            }

            if (dbValue is int)
            {
                return dbValue;
            }

            return Convert.ToInt32(dbValue);
        }

        public object ToDB(object clrValue)
        {
            return clrValue;
        }

        public Type DbType { get; private set; }
    }
}