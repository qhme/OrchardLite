namespace Orchard.Infrastructure
{
    /// <summary>
    /// 初始任务
    /// 注意：IStartup没有使用依赖注入
    /// </summary>
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
