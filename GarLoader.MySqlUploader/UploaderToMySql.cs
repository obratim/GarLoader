using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GarLoader.Engine;
using Microsoft.Extensions.Logging;
using SqlWorker;

namespace GarLoader.MySqlUploader
{
    class UploaderToMySql : IUploader
    {
        private readonly UpdaterConfiguration _configuration;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public UploaderToMySql(UpdaterConfiguration configuration, Microsoft.Extensions.Logging.ILogger<UploaderToMySql> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void CleanUp()
        {
            _logger.LogInformation("Завершение ...");
            using var sw = new MySqlWorker(_configuration.ConnectionString);
            sw.DefaultExecutionTimeout = 600_000;
            _logger.LogInformation("Удаление неактульных записей из address_object_param...");
            sw.Exec(@"
            DELETE FROM address_object_param
            WHERE EXISTS (
                select '' from address_object a
                where a.object_id = address_object_param.object_id and (a.is_active = 0 or a.is_actual = 0 or a.start_date > NOW() or a.end_date < NOW())
            )
            ");
            _logger.LogInformation("Удаление неактульных записей из address_object_param завершено");
            _logger.LogInformation("Удаление неактульных записей из administrative_hierarchy ...");
            sw.Exec(@"
            DELETE FROM administrative_hierarchy
            WHERE EXISTS (
                select '' from address_object a
                where a.object_id = administrative_hierarchy.object_id and (a.is_active = 0 or a.is_actual = 0 or a.start_date > NOW() or a.end_date < NOW())
            )
            ");
            _logger.LogInformation("Удаление неактульных записей из administrative_hierarchy завершено");
            _logger.LogInformation("Удаление неактульных записей из municipal_hierarchy ...");
            sw.Exec(@"
            DELETE FROM municipal_hierarchy
            WHERE EXISTS (
                select '' from address_object a
                where a.object_id = municipal_hierarchy.object_id and (a.is_active = 0 or a.is_actual = 0 or a.start_date > NOW() or a.end_date < NOW())
            )
            ");
            _logger.LogInformation("Удаление неактульных записей из municipal_hierarchy завершено");
            _logger.LogInformation("Удаление неактульных записей из address_object ...");
            sw.Exec(@"
            DELETE FROM address_object
            WHERE is_active = 0 or is_actual = 0 or start_date > NOW() or end_date < NOW()
            ");
            _logger.LogInformation("Удаление неактульных записей из address_object завершено");
        }

        public void InsertAddressObjectItems<T>(IEnumerable<T> items)
            =>
                Inserter<T>.InsertItems(_configuration.ConnectionString, items);

        public void UpdateDb(int updateId, string description, DateTime processStartedDt, string urlDataFull, string urlDataDelta)
        {
            throw new NotImplementedException();
        }
    }

    static class Inserter<T>
    {
        private static readonly Action<string, IEnumerable<T>> InserterImplementation;
        private static readonly DateTime Now = DateTime.Now;

        static Inserter()
        {
            InserterImplementation = typeof(T) switch
            {
                {} when typeof(T) == typeof(AddressObjectType) => InsertAddressObjectTypes,
                {} when typeof(T) == typeof(ObjectLevel) => InsertObjectLevels,
                {} when typeof(T) == typeof(OperationType) => InsertOperationType,
                {} when typeof(T) == typeof(ParamType) => InsertParamTypes,
                {} when typeof(T) == typeof(AddressObject) => BulkInsertAddressObjects,
                {} when typeof(T) == typeof(Parameter) => BulkInsertAddressObjectParameters,
                {} when typeof(T) == typeof(AdministrativeHierarchyItem) => BulkInsertAdministrativeHierarchy,
                {} when typeof(T) == typeof(MunicipalHierarchyItem) => BulkInsertMunicipalHierarchy,
                _ => null,
            };
        }


        public static void InsertItems(string connectionString, IEnumerable<T> items)
        {
            if (InserterImplementation == null)
                throw new NotImplementedException ($"Нет реализации для типа {typeof(T).FullName}");
            InserterImplementation(connectionString, items);
        }

        private static void InsertAddressObjectTypes(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<AddressObjectType>)items)
            {
                sw.Exec(
                    @"
INSERT INTO address_object_type (id, level, shortname, name, description, update_date, start_date, end_date) VALUES
(@id, @level, @sname, @name, @desc, @ud, @sd, @ed)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "level", item.Level },
                        { "sname", item.ShortName },
                        { "name", item.Name },
                        { "desc", item.Description },
                        { "ud", item.UpdateDate },
                        { "sd", item.StartDate },
                        { "ed", item.EndDate },
                    });
            }
        }

        private static void InsertObjectLevels(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<ObjectLevel>)items)
            {       
                sw.Exec(
                    @"
INSERT INTO object_level (level, name, shortname, update_date, start_date, end_date) VALUES
(@level, @name, @sname, @ud, @sd, @ed)",
                    new SwParameters
                    {
                        { "level", item.Level },
                        { "sname", item.ShortName },
                        { "name", item.Name },
                        { "ud", item.UpdateDate },
                        { "sd", item.StartDate },
                        { "ed", item.EndDate },
                    });
            }
        }

        private static void InsertOperationType(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<OperationType>)items)
            {       
                sw.Exec(
                    @"
INSERT INTO operation_type (id, name, shortname, description, update_date, start_date, end_date) VALUES
(@id, @name, @sname, @desc, @ud, @sd, @ed)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "sname", item.ShortName },
                        { "name", item.Name },
                        { "desc", item.Description },
                        { "ud", item.UpdateDate },
                        { "sd", item.StartDate },
                        { "ed", item.EndDate },
                    });
            }
        }

        private static void InsertParamTypes(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<ParamType>)items)
            {       
                sw.Exec(
                    @"
INSERT INTO param_type (id, name, code, description, update_date, start_date, end_date) VALUES
(@id, @name, @code, @desc, @ud, @sd, @ed)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "name", item.Name },
                        { "code", item.Code },
                        { "desc", item.Description },
                        { "ud", item.UpdateDate },
                        { "sd", item.StartDate },
                        { "ed", item.EndDate },
                    });
            }
        }

        private static string PrepareInsertStatement<TT>(string tableName, ICollection<(string name, Func<TT, object> valueGetter)> fields, int rowsCount)
        {
            var sb = new StringBuilder(@"INSERT INTO ");
            sb.Append(tableName);
            sb.Append(" (");
            var delimiter = "";
            foreach (var column in fields)
            {
                sb.Append(delimiter);
                delimiter = ", ";
                sb.Append(column.name);
            }
            sb.Append(") VALUES ");
            delimiter = "";
            for (var i = 0; i < rowsCount; ++i)
            {
                sb.Append(delimiter);
                delimiter = ", ";
                sb.Append('(');
                var tmpDelimiter = "";
                foreach (var column in fields)
                {
                    sb.Append(tmpDelimiter);
                    tmpDelimiter = ", ";
                    sb.Append('@');
                    sb.Append(column.name);
                    sb.Append(i);
                }
                sb.Append(')');
            }

            return sb.ToString();
        }

        private static void GenericInsertBulk<TT>(string connectionString, IEnumerable<TT> items, string tableName, ICollection<(string name, Func<TT, object> valueGetter)> fields)
        {
            using var enumerator = items.GetEnumerator();
            using var sw = new MySqlWorker(connectionString);
            int i;
            var parameters = new SwParameters();
            const int bulkNumber = 50;
            var insertCommand = PrepareInsertStatement(tableName, fields, bulkNumber);
            do
            {
                parameters.Clear();
                for (i = 0; i < bulkNumber; ++i)
                {
                    if (!enumerator.MoveNext())
                        break;
                    foreach (var column in fields)
                    {
                        parameters.Add($"{column.name}{i}", column.valueGetter(enumerator.Current));
                    }
                }

                if (i == bulkNumber)
                    sw.Exec(insertCommand, parameters);
            }
            while (i == bulkNumber);

            if (i > 0)
                sw.Exec(PrepareInsertStatement(tableName, fields, i), parameters);
        }


        private static void InsertAddressObjects(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<AddressObject>)items)
            {
                if (item.IsActive != 1 || item.IsActual != 1 || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
                sw.Exec(
                    @"
INSERT INTO address_object (id, object_id, object_guid, change_id, name, region, type_name, level, operation_type_id, prev_id, next_id, update_date, start_date, end_date) VALUES
(@id, @obj_id, @obj_guid, @change_id, @name, @region, @type_name, @level, @oper_type, @prev_id, @next_id, @ud, @sd, @ed)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "obj_id", item.ObjectId },
                        { "obj_guid", item.ObjectGuid },
                        { "change_id", item.ChangedTransactionId },
                        { "name", item.Name ?? "<NULL>" },
                        { "region", item.Region },
                        { "type_name", item.TypeName },
                        { "level", item.Level },
                        { "oper_type", item.OperationTypeId },
                        { "prev_id", item.PrevId },
                        { "next_id", item.NextId },
                        { "ud", item.UpdateDate },
                        { "sd", item.StartDate },
                        { "ed", item.EndDate },
                    });
            }
        }

        private static void InsertAddressObjectParameters(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<Parameter>)items)
            {
                if (item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
                sw.Exec(
                    @"
INSERT INTO address_object_param (id, object_id, change_id, change_id_end, type_id, value, update_date, start_date, end_date) VALUES
(@id, @object_id, @change_id, @change_id_end, @type_id, @value, @update_date, @start_date, @end_date)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "object_id", item.ObjectId },
                        { "change_id", item.ChangeTransactionId },
                        { "change_id_end", item.ChangeTransactionIdEnd },
                        { "type_id", item.TypeId },
                        { "value", item.Value },
                        { "update_date", item.UpdateDate },
                        { "start_date", item.StartDate },
                        { "end_date", item.EndDate },
                    });
            }
        }

        private static void InsertAdministrativeHierarchy(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<AdministrativeHierarchyItem>)items)
            {
                if (item.IsActive != 1 || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
                sw.Exec(
                    @"
INSERT INTO administrative_hierarchy (id, object_id, parent_object_id, change_id, region_code, area_code, city_code, place_code, plan_code, street_code, prev_id, next_id, update_date, start_date, end_date) VALUES
(@id, @object_id, @parent_object_id, @change_id, @region_code, @area_code, @city_code, @place_code, @plan_code, @street_code, @prev_id, @next_id, @update_date, @start_date, @end_date)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "object_id", item.ObjectId },
                        { "parent_object_id", item.ParentObjectId },
                        { "change_id", item.ChangeTransactionId },
                        { "region_code", item.RegionCode },
                        { "area_code", item.AreaCode },
                        { "city_code", item.CityCode },
                        { "place_code", item.PlaceCode },
                        { "plan_code", item.PlanCode },
                        { "street_code", item.StreetCode },
                        { "prev_id", item.PrevId },
                        { "next_id", item.NextId },
                        { "update_date", item.UpdateDate },
                        { "start_date", item.StartDate },
                        { "end_date", item.EndDate },
                    });
            }
        }

        private static void InsertMunicipalHierarchy(string connectionString, IEnumerable<T> items)
        {
            using var sw = new MySqlWorker(connectionString);
            foreach (var item in (IEnumerable<MunicipalHierarchyItem>)items)
            {
                if (item.IsActive != 1 || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
                sw.Exec(
                    @"
INSERT INTO municipal_hierarchy (id, object_id, parent_object_id, change_id, oktmo, prev_id, next_id, update_date, start_date, end_date) VALUES
(@id, @object_id, @parent_object_id, @change_id, @oktmo, @prev_id, @next_id, @update_date, @start_date, @end_date)",
                    new SwParameters
                    {
                        { "id", item.Id },
                        { "object_id", item.ObjectId },
                        { "parent_object_id", item.ParentObjectId },
                        { "change_id", item.ChangeTransactionId },
                        { "oktmo", item.OKTMO },
                        { "prev_id", item.PrevId },
                        { "next_id", item.NextId },
                        { "update_date", item.UpdateDate },
                        { "start_date", item.StartDate },
                        { "end_date", item.EndDate },
                    });
            }
        }

        private static void BulkInsertAddressObjects(string connectionString, IEnumerable<T> items)
            =>
                GenericInsertBulk(
                    connectionString,
                    ((IEnumerable<AddressObject>)items),//.Where(item => /*item.IsActive == 1 &&*/ item.IsActual == 1 && item.StartDate <= Now && item.EndDate >= Now),
                    "address_object",
                    new (string, Func<AddressObject, object>)[] {
                        ("id", x => x.Id),
                        ("object_id", x => x.ObjectId),
                        ("object_guid", x => x.ObjectGuid.ToString()),
                        ("change_id", x => x.ChangedTransactionId),
                        ("name", x => x.Name ?? "<NULL>"),
                        ("region", x => x.Region),
                        ("type_name", x => x.TypeName),
                        ("level", x => x.Level),
                        ("operation_type_id", x => x.OperationTypeId),
                        ("prev_id", x => x.PrevId),
                        ("next_id", x => x.NextId),
                        ("update_date", x => x.UpdateDate),
                        ("start_date", x => x.StartDate),
                        ("end_date", x => x.EndDate),
                        ("is_active", x => x.IsActive),
                        ("is_actual", x => x.IsActual),
                    });
        
        private static void BulkInsertAddressObjectParameters(string connectionString, IEnumerable<T> items)
            =>
                GenericInsertBulk(
                    connectionString,
                    ((IEnumerable<Parameter>)items).Where(item => !(item.StartDate > Now || item.EndDate < Now)),
                    "address_object_param",
                    new (string, Func<Parameter, object>)[] {
                        ("id", x => x.Id),
                        ("object_id", x => x.ObjectId),
                        ("change_id", x => x.ChangeTransactionId),
                        ("change_id_end", x => x.ChangeTransactionIdEnd),
                        ("type_id", x => x.TypeId),
                        ("value", x => x.Value),
                        ("update_date", x => x.UpdateDate),
                        ("start_date", x => x.StartDate),
                        ("end_date", x => x.EndDate),
                    }
                );
        private static void BulkInsertAdministrativeHierarchy(string connectionString, IEnumerable<T> items)
            =>
                GenericInsertBulk(
                    connectionString,
                    ((IEnumerable<AdministrativeHierarchyItem>)items).Where(item => !(item.IsActive != 1 || item.StartDate > Now || item.EndDate < Now)),
                    "administrative_hierarchy",
                    new (string, Func<AdministrativeHierarchyItem, object>)[] {
                        ("id", x => x.Id),
                        ("object_id", x => x.ObjectId),
                        ("parent_object_id", x => x.ParentObjectId),
                        ("change_id", x => x.ChangeTransactionId),
                        ("region_code", x => x.RegionCode),
                        ("area_code", x => x.AreaCode),
                        ("city_code", x => x.CityCode),
                        ("place_code", x => x.PlaceCode),
                        ("plan_code", x => x.PlanCode),
                        ("street_code", x => x.StreetCode),
                        ("prev_id", x => x.PrevId),
                        ("next_id", x => x.NextId),
                        ("update_date", x => x.UpdateDate),
                        ("start_date", x => x.StartDate),
                        ("end_date", x => x.EndDate),
                    }
                );
        private static void BulkInsertMunicipalHierarchy(string connectionString, IEnumerable<T> items)
            =>
                GenericInsertBulk(
                    connectionString,
                    ((IEnumerable<MunicipalHierarchyItem>)items).Where(item => !(item.IsActive != 1 || item.StartDate > Now || item.EndDate < Now)),
                    "municipal_hierarchy",
                    new (string, Func<MunicipalHierarchyItem, object>)[] {
                        ("id", x => x.Id),
                        ("object_id", x => x.ObjectId),
                        ("parent_object_id", x => x.ParentObjectId),
                        ("change_id", x => x.ChangeTransactionId),
                        ("oktmo", x => x.OKTMO),
                        ("prev_id", x => x.PrevId),
                        ("next_id", x => x.NextId),
                        ("update_date", x => x.UpdateDate),
                        ("start_date", x => x.StartDate),
                        ("end_date", x => x.EndDate),
                    }
                );


    }
}
