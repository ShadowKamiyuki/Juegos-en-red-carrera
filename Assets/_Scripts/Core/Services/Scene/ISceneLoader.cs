using System;
using System.Threading.Tasks;

public interface ISceneLoader
{
    Task ProcessRequest(LoadingRequest request, Action<float> onProgress = null);
}
