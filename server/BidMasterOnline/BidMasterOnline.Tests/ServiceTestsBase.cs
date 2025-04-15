using AutoMapper;
using BidMasterOnline.Application.MappingProfiles;
using BidMasterOnline.Application.RepositoryContracts;
using Moq;

namespace BidMasterOnline.Tests
{
    /// <summary>
    /// Base test class for service unit tests.
    /// </summary>
    public abstract class ServiceTestsBase : BidMasterOnlineTestsBase
    {
        protected readonly Mock<IAsyncRepository> repositoryMock;
        protected readonly IMapper mapper;

        public ServiceTestsBase()
        {
            repositoryMock = new Mock<IAsyncRepository>();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });

            mapper = mapperConfiguration.CreateMapper();
        }
    }
}
