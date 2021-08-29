using System;
using System.Collections.Generic;
using GarLoader.Engine;

namespace GarLoader.MySqlUploader
{
    class UploaderToMySql : IUploader
    {
        public void CleanUp()
        {
            //throw new NotImplementedException();
        }

        public void InsertAddressObjectTypes(IEnumerable<AddressObjectType> items)
        {
            throw new NotImplementedException();
        }

        public void UpdateDb(int updateId, string description, DateTime processStartedDt, string urlDataFull, string urlDataDelta)
        {
            throw new NotImplementedException();
        }
    }
}
