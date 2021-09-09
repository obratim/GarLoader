using System;
using System.Collections.Generic;
using System.Text;
using GarLoader.Engine;
using SqlWorker;

namespace GarLoader.MySqlUploader
{
    class UploaderToMySql : IUploader
    {
        private readonly UpdaterConfiguration _configuration;

        public UploaderToMySql(UpdaterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CleanUp()
        {
            //throw new NotImplementedException();
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
                {} when typeof(T) == typeof(AddressObject) => InsertAddressObjects,
                {} when typeof(T) == typeof(Parameter) => InsertAddressObjectParameters,
                {} when typeof(T) == typeof(AdministrativeHierarchyItem) => InsertAdministrativeHierarchy,
                {} when typeof(T) == typeof(MunicipalHierarchyItem) => InsertMunicipalHierarchy,
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
                if (!item.IsActive || item.StartDate > Now || item.EndDate < Now)
                    continue;

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
                if (!item.IsActive || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
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
                if (!item.IsActive || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
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
                if (!item.IsActive || item.StartDate > Now || item.EndDate < Now)
                    continue;
                    
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


    }
}
