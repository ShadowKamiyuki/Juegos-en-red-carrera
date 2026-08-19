using System.Threading.Tasks;

public interface IFadeService
{
    Task FadeToBlackAsync();
    Task FadeFromBlackAsync();
}
