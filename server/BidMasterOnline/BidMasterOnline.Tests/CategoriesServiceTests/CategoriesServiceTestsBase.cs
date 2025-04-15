using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;

namespace BidMasterOnline.Tests.CategoriesServiceTests
{
    /// <summary>
    /// Base test class for CategoriesService unit tests.
    /// </summary>
    public abstract class CategoriesServiceTestsBase : ServiceTestsBase
    {
        protected readonly ICategoriesService service;

        public CategoriesServiceTestsBase()
        {
            service = new CategoriesService(this.repositoryMock.Object, this.mapper);
        }
    }
}
