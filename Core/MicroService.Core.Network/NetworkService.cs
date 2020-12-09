using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using ProtoBuf.ServiceModel;

namespace MicroService.Core.Network
{
    public class NetworkService : INetworkService
    {
        private readonly CompositionContainer container;
        private readonly CustomBinding binding;

        public NetworkService(CompositionContainer container)
        {
            this.container = container;

            var tcpBind = new NetTcpContextBinding(SecurityMode.Transport, true)
            {
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                MaxBufferPoolSize = 2147483647,
                ListenBacklog = 10000,
                MaxConnections = 10000,
                ReceiveTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(1),
                ReliableSession = { InactivityTimeout = TimeSpan.FromMinutes(1) },
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1)
            };
            tcpBind.Security.Message.ClientCredentialType = MessageCredentialType.None;
            tcpBind.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            tcpBind.ReaderQuotas.MaxStringContentLength = 500000;
            binding = new CustomBinding(tcpBind);
            var sbes = binding.Elements.FindAll<SecurityBindingElement>();
            foreach (var sbe in sbes)
            {
                sbe.IncludeTimestamp = false;
                sbe.LocalClientSettings.DetectReplays = false;
                sbe.LocalClientSettings.ReconnectTransportOnFailure = true;
                sbe.LocalClientSettings.MaxClockSkew = TimeSpan.FromHours(6);
                sbe.LocalClientSettings.SessionKeyRenewalInterval = TimeSpan.MaxValue;
                sbe.LocalServiceSettings.DetectReplays = false;
                sbe.LocalServiceSettings.ReconnectTransportOnFailure = true;
                sbe.LocalServiceSettings.MaxClockSkew = TimeSpan.FromHours(6);
                sbe.LocalServiceSettings.SessionKeyRenewalInterval = TimeSpan.MaxValue;
            }
        }

        public void Initialize()
        {
            
        }

        public void Use<T>(Action<T> action, object handler, Uri uri)
        {
            var endpoint = new EndpointAddress(uri, new DnsEndpointIdentity("ЦОД"));
            using (var factory = new DuplexChannelFactory<T>(handler.GetType(), binding, endpoint))
            {
                factory.Endpoint.Behaviors.Add(new ProtoEndpointBehavior());
                factory.Credentials.ClientCertificate.Certificate = new X509Certificate2(@"DataCenter.pfx", "ЦОД");
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                factory.Open();
                var channel = factory.CreateChannel(new InstanceContext(handler));
                var success = false;
                try
                {
                    action(channel);
                    ((IClientChannel)channel).Close();
                    factory.Close();
                    success = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (!success)
                    {
                        ((IClientChannel)channel).Abort();
                        factory.Abort();
                    }
                }
            }
        }

        public void CreateServiceHost(IService svc)
        {
            try
            {
                var uri = new Uri($"net.tcp://{GetIpAddress()}:9091/I{svc.GetType().Name}/");
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var host = new ServiceHost(svc, uri);
                host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                host.Description.Behaviors.Add(new ServiceMetadataBehavior());

                host.Faulted += OnFaulted;
                host.Closing += OnClosing;

                host.Credentials.ServiceCertificate.Certificate = new X509Certificate2(@"DataCenter.pfx", "ЦОД");
                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
                var endpoint = host.AddServiceEndpoint(svc.GetType().GetInterfaces()[0], binding, uri);
                endpoint.Behaviors.Add(new ProtoEndpointBehavior());

                var errorHandler = new ErrorHandler();
                foreach (var channelDispatcher in host.ChannelDispatchers.Cast<ChannelDispatcher>())
                {
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }

                host.Open();

                Console.WriteLine("Service started : {0}", uri);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
        }

        private string GetIpAddress()
        {
            string ip = null;
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var ua in ni.GetIPProperties().UnicastAddresses.Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork && ua.IsDnsEligible))
                {
                    ip = ua.Address.ToString();
                    break;
                }
            }
            return ip;
        }

        private void OnFaulted(object sender, EventArgs eventArgs)
        {
            var channel = ((ICommunicationObject)sender);
            channel.Abort();
            Console.WriteLine("WCF Faulted: {0}", sender);
        }

        private void OnClosing(object sender, EventArgs eventArgs)
        {
            var channel = ((ICommunicationObject)sender);
            channel.Close();
            Console.WriteLine("WCF Closed: {0}", sender);
        }

        public void Dispose()
        {
            
        }

        public FindResponse Discovery<T>(int timeout) where T :  IService
        {
            var endpoint = new UdpDiscoveryEndpoint();

            var discoveryClient = new DiscoveryClient(endpoint);

            var findCriteria = new FindCriteria(typeof(T))
            {
                Duration = TimeSpan.FromMilliseconds(timeout)
            };

            var responce = discoveryClient.Find(findCriteria);

            discoveryClient.Close();

            return responce;
        }

    }
}
