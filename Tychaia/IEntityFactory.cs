﻿// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using Tychaia.Data;

namespace Tychaia
{
    public interface IEntityFactory
    {
        EnemyEntity CreateEnemyEntity(Cell cell);
    }
}

