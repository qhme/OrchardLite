
using Orchard.Logging;
namespace Orchard
{
    /// <summary>
    /// 单实例为每个请求
    /// </summary>
    public interface IDependency
    {
    }

    /// <summary>
    /// 单例为整个程序
    /// </summary>
    public interface ISingletonDependency : IDependency
    {

    }

    /// <summary>
    /// 单例为每个work
    /// </summary>
    public interface IUnitOfWorkDependency : IDependency
    {

    }

    /// <summary>
    /// 单实例为每次使用
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }


    public abstract class Component : IDependency
    {
        protected Component()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
    }

}
