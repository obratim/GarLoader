using System;

namespace GarLoader.Engine
{
    public class UpdateException : Exception
    {
		public UpdateException(string message = null, Exception innerException = null)
			: base(message, innerException) { }
    }

	public class PreviousUpdateFailedException : UpdateException
	{
		public PreviousUpdateFailedException(string message = null, Exception innerException = null)
			: base(message, innerException) { }
	}

	public class CurrentDataExpiredException : UpdateException
	{
		public CurrentDataExpiredException(string message = null, Exception innerException = null)
			: base(message, innerException) { }
	}
}
