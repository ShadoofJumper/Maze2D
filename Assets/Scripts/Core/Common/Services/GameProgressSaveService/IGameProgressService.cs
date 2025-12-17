namespace Plugins.Core.Common.Services.GameProgressSaveService
{
    public interface IGameProgressService
    {
        void Save(GameResultData result);
        GameResultData Load();
        bool HasSave();
        void Clear();
    }
}