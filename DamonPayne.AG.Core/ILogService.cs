using System;


namespace DamonPayne.AG.Core
{

    public interface ILogService
    {
        void Log(DamonPayne.AG.Core.DataTypes.LogMessage m);
    }
}