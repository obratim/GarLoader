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

        public void InsertAddressObjectTypes(IEnumerable<AddressObjectType> items)
        {
            using var sw = new MySqlWorker(_configuration.ConnectionString);
            //var sb = new StringBuilder(1024*1024);
            //sb.Append("INSERT INTO address_object_type (id, level, short_name, name, description, update_date, start_date, end_date, is_active) VALUES");
            //using var enumerator = items.GetEnumerator();
            //if (!enumerator.MoveNext())
            //    return;
            //sb.Append()
            foreach (var item in items)
            {
                sw.Exec(
                    @"
INSERT INTO address_object_type (id, level, short_name, name, description, update_date, start_date, end_date, is_active) VALUES
(@id, @level, @sname, @name, @desc, @ud, @sd, @ed, @isactive)",
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
                        { "isactive", item.IsActive },
                    });
            }
        }

        public void UpdateDb(int updateId, string description, DateTime processStartedDt, string urlDataFull, string urlDataDelta)
        {
            throw new NotImplementedException();
        }
    }
}
