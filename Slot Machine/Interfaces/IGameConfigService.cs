using Slot_Machine.Collections;

namespace Slot_Machine.Interfaces
{
    public interface IGameConfigService
    {
        Task<GameConfig> GetConfigurationAsync(string configid= "default");
        Task UpdateConfigurationAsync(UpdateConfigurationRequest request);
    }
}
