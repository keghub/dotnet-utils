using Topshelf;

namespace EMG.Utilities.Hosting
{
    public interface IService
    {
        bool OnStart(HostControl control);

        bool OnStop(HostControl control);
    }
}