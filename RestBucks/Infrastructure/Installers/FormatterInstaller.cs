using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;

namespace RestBucks.Infrastructure.Installers
{
    public class FormatterInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromThisAssembly()
                                   .BasedOn<MediaTypeFormatter>()
                                   .WithService.Base());

            container.Register(AllTypes.FromAssemblyContaining<XmlMediaTypeFormatter>()
                                   .BasedOn<MediaTypeFormatter>()
                                   .WithService.Base());
        }
    }
}