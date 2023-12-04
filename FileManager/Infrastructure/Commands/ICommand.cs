namespace FileManager.Infrastructure.Commands
{
    public interface ICommand
    {
        void Execute();         // we don`t undo commands. We use `em only for deferred execution
    }
}
