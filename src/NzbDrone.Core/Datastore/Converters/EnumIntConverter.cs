using System;
using Marr.Data.Converters;
using Marr.Data.Mapping;

namespace NzbDrone.Core.Datastore.Converters
{
    public class EnumIntConverter : IConverter
    {
        public Type DbType
        {
            get
            {
                return typeof(int);
            }
        }

        public object FromDB(ConverterContext context)
        {
            if (context.DbValue != null && context.DbValue != DBNull.Value)
            {
                return Enum.ToObject(context.ColumnMap.FieldType, (long)context.DbValue);
            }

            return null;
        }

        public object FromDB(ColumnMap map, object dbValue)
        {
            return FromDB(new ConverterContext { ColumnMap = map, DbValue = dbValue });
        }

        public object ToDB(object clrValue)
        {
            if (clrValue != null)
            {
                return (int)clrValue;
            }

            return DBNull.Value;
        }
    }
}