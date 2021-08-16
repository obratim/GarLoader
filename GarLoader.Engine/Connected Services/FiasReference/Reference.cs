//------------------------------------------------------------------------------
// <автоматически создаваемое>
//     Этот код создан программой.
//     //
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторного создания кода.
// </автоматически создаваемое>
//------------------------------------------------------------------------------

namespace FiasReference
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DownloadFileInfo", Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx")]
    public partial class DownloadFileInfo : object
    {
        
        private int VersionIdField;
        
        private string TextVersionField;
        
        private string FiasCompleteDbfUrlField;
        
        private string FiasCompleteXmlUrlField;
        
        private string FiasDeltaDbfUrlField;
        
        private string FiasDeltaXmlUrlField;
        
        private string Kladr4ArjUrlField;
        
        private string Kladr47ZUrlField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int VersionId
        {
            get
            {
                return this.VersionIdField;
            }
            set
            {
                this.VersionIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string TextVersion
        {
            get
            {
                return this.TextVersionField;
            }
            set
            {
                this.TextVersionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string FiasCompleteDbfUrl
        {
            get
            {
                return this.FiasCompleteDbfUrlField;
            }
            set
            {
                this.FiasCompleteDbfUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string FiasCompleteXmlUrl
        {
            get
            {
                return this.FiasCompleteXmlUrlField;
            }
            set
            {
                this.FiasCompleteXmlUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string FiasDeltaDbfUrl
        {
            get
            {
                return this.FiasDeltaDbfUrlField;
            }
            set
            {
                this.FiasDeltaDbfUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string FiasDeltaXmlUrl
        {
            get
            {
                return this.FiasDeltaXmlUrlField;
            }
            set
            {
                this.FiasDeltaXmlUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string Kladr4ArjUrl
        {
            get
            {
                return this.Kladr4ArjUrlField;
            }
            set
            {
                this.Kladr4ArjUrlField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string Kladr47ZUrl
        {
            get
            {
                return this.Kladr47ZUrlField;
            }
            set
            {
                this.Kladr47ZUrlField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx", ConfigurationName="FiasReference.DownloadServiceSoap")]
    public interface DownloadServiceSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx/GetLastDownloadFileI" +
            "nfo", ReplyAction="*")]
        System.Threading.Tasks.Task<FiasReference.GetLastDownloadFileInfoResponse> GetLastDownloadFileInfoAsync(FiasReference.GetLastDownloadFileInfoRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx/GetAllDownloadFileIn" +
            "fo", ReplyAction="*")]
        System.Threading.Tasks.Task<FiasReference.GetAllDownloadFileInfoResponse> GetAllDownloadFileInfoAsync(FiasReference.GetAllDownloadFileInfoRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetLastDownloadFileInfoRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetLastDownloadFileInfo", Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx", Order=0)]
        public FiasReference.GetLastDownloadFileInfoRequestBody Body;
        
        public GetLastDownloadFileInfoRequest()
        {
        }
        
        public GetLastDownloadFileInfoRequest(FiasReference.GetLastDownloadFileInfoRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class GetLastDownloadFileInfoRequestBody
    {
        
        public GetLastDownloadFileInfoRequestBody()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetLastDownloadFileInfoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetLastDownloadFileInfoResponse", Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx", Order=0)]
        public FiasReference.GetLastDownloadFileInfoResponseBody Body;
        
        public GetLastDownloadFileInfoResponse()
        {
        }
        
        public GetLastDownloadFileInfoResponse(FiasReference.GetLastDownloadFileInfoResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx")]
    public partial class GetLastDownloadFileInfoResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public FiasReference.DownloadFileInfo GetLastDownloadFileInfoResult;
        
        public GetLastDownloadFileInfoResponseBody()
        {
        }
        
        public GetLastDownloadFileInfoResponseBody(FiasReference.DownloadFileInfo GetLastDownloadFileInfoResult)
        {
            this.GetLastDownloadFileInfoResult = GetLastDownloadFileInfoResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAllDownloadFileInfoRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetAllDownloadFileInfo", Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx", Order=0)]
        public FiasReference.GetAllDownloadFileInfoRequestBody Body;
        
        public GetAllDownloadFileInfoRequest()
        {
        }
        
        public GetAllDownloadFileInfoRequest(FiasReference.GetAllDownloadFileInfoRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class GetAllDownloadFileInfoRequestBody
    {
        
        public GetAllDownloadFileInfoRequestBody()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetAllDownloadFileInfoResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetAllDownloadFileInfoResponse", Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx", Order=0)]
        public FiasReference.GetAllDownloadFileInfoResponseBody Body;
        
        public GetAllDownloadFileInfoResponse()
        {
        }
        
        public GetAllDownloadFileInfoResponse(FiasReference.GetAllDownloadFileInfoResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://fias.nalog.ru/WebServices/Public/DownloadService.asmx")]
    public partial class GetAllDownloadFileInfoResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public FiasReference.DownloadFileInfo[] GetAllDownloadFileInfoResult;
        
        public GetAllDownloadFileInfoResponseBody()
        {
        }
        
        public GetAllDownloadFileInfoResponseBody(FiasReference.DownloadFileInfo[] GetAllDownloadFileInfoResult)
        {
            this.GetAllDownloadFileInfoResult = GetAllDownloadFileInfoResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    public interface DownloadServiceSoapChannel : FiasReference.DownloadServiceSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    public partial class DownloadServiceSoapClient : System.ServiceModel.ClientBase<FiasReference.DownloadServiceSoap>, FiasReference.DownloadServiceSoap
    {
        
    /// <summary>
    /// Реализуйте этот разделяемый метод для настройки конечной точки службы.
    /// </summary>
    /// <param name="serviceEndpoint">Настраиваемая конечная точка</param>
    /// <param name="clientCredentials">Учетные данные клиента.</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public DownloadServiceSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(DownloadServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), DownloadServiceSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DownloadServiceSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(DownloadServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DownloadServiceSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(DownloadServiceSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DownloadServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<FiasReference.GetLastDownloadFileInfoResponse> FiasReference.DownloadServiceSoap.GetLastDownloadFileInfoAsync(FiasReference.GetLastDownloadFileInfoRequest request)
        {
            return base.Channel.GetLastDownloadFileInfoAsync(request);
        }
        
        public System.Threading.Tasks.Task<FiasReference.GetLastDownloadFileInfoResponse> GetLastDownloadFileInfoAsync()
        {
            FiasReference.GetLastDownloadFileInfoRequest inValue = new FiasReference.GetLastDownloadFileInfoRequest();
            inValue.Body = new FiasReference.GetLastDownloadFileInfoRequestBody();
            return ((FiasReference.DownloadServiceSoap)(this)).GetLastDownloadFileInfoAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<FiasReference.GetAllDownloadFileInfoResponse> FiasReference.DownloadServiceSoap.GetAllDownloadFileInfoAsync(FiasReference.GetAllDownloadFileInfoRequest request)
        {
            return base.Channel.GetAllDownloadFileInfoAsync(request);
        }
        
        public System.Threading.Tasks.Task<FiasReference.GetAllDownloadFileInfoResponse> GetAllDownloadFileInfoAsync()
        {
            FiasReference.GetAllDownloadFileInfoRequest inValue = new FiasReference.GetAllDownloadFileInfoRequest();
            inValue.Body = new FiasReference.GetAllDownloadFileInfoRequestBody();
            return ((FiasReference.DownloadServiceSoap)(this)).GetAllDownloadFileInfoAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.DownloadServiceSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.DownloadServiceSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Не удалось найти конечную точку с именем \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.DownloadServiceSoap))
            {
                return new System.ServiceModel.EndpointAddress("https://fias.nalog.ru/WebServices/Public/DownloadService.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.DownloadServiceSoap12))
            {
                return new System.ServiceModel.EndpointAddress("https://fias.nalog.ru/WebServices/Public/DownloadService.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Не удалось найти конечную точку с именем \"{0}\".", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            DownloadServiceSoap,
            
            DownloadServiceSoap12,
        }
    }
}
