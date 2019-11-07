using System.Threading.Tasks;

namespace Twino.Ioc.Pool
{
    public interface IServicePool
    {
        /// <summary>
        /// Gets and locks a service instance.
        /// Return type should be PoolServiceDescriptor<TService> 
        /// </summary>
        /// <returns></returns>
        Task<PoolServiceDescriptor> GetAndLock(IContainerScope scope = null);

        /// <summary>
        /// Releases pool item by instance
        /// </summary>
        /// <returns></returns>
        void ReleaseInstance(object instance);

        /// <summary>
        /// Releases pool item for re-using. 
        /// </summary>
        void Release(PoolServiceDescriptor descriptor);
    }
}