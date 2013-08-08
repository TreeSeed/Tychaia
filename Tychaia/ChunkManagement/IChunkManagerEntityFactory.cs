using System;

namespace Tychaia
{
    public interface IChunkManagerEntityFactory
    {
        ChunkManagerEntity CreateChunkManagerEntity(TychaiaGameWorld gameWorld);
    }
}

