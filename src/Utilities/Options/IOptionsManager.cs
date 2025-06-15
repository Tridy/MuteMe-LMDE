namespace Utilities;

public interface IOptionsManager
{
    Options GetOptions();
    void SaveOptions(Options options);
}