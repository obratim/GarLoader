using System;
using System.Collections.Generic;

namespace GarLoader.Engine
{
    public interface IUploader
    {
		//void PutAddressObjectsChanges(string entryName, IEnumerable<FiasTypes.AddressObject> addressObjects);
		//void PutHousesChanges(string entryName, IEnumerable<FiasTypes.House> houses);
		//void PutHouseIntsChanges(string entryName, IEnumerable<FiasTypes.HouseInterval> houseIntervals);
		//void PutLandmarksChanges(string entryName, IEnumerable<FiasTypes.Landmark> landmarks);

		void InsertAddressObjectItems<T>(IEnumerable<T> items);
		
		void UpdateDb(
			int updateId,
			string description,
			DateTime processStartedDt,
			string urlDataFull,
			string urlDataDelta/*,
			IEnumerable<FiasTypes.AddressObject> addressObjectsToDelete,
			IEnumerable<FiasTypes.House> housesToDelete,
			IEnumerable<FiasTypes.HouseInterval> houseIntervalsToDelete,
			IEnumerable<FiasTypes.Landmark> landmarksToDelete*/);

		void CleanUp();
	}
}
