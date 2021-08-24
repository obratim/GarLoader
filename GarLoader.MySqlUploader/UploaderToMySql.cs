using System;
using System.Collections.Generic;
using GarLoader.Engine;

namespace GarLoader.MySqlUploader
{
    class UploaderToMySql : IUploader
    {
        public bool CheckRegion()
        {
            return true;
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetAddressObjectGuids()
        {
            yield break;
            throw new NotImplementedException();
        }

        public (int, string) GetLastUpdateIdAndDateTime()
        {
            return default;
            throw new NotImplementedException();
        }

        public void InitializeTempTables()
        {
            //throw new NotImplementedException();
        }

        public void UpdateDb(int updateId, string description, DateTime processStartedDt, string urlDataFull, string urlDataDelta)
        {
            //throw new NotImplementedException();
        }
    }
}
