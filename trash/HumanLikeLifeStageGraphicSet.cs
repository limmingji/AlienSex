using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse
{
    
    public class HumanLikeLifeStageGraphicSet
    {
        public HumanLikeLifeStageGraphicSet(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public GraphicMeshSet HumanLikeLifeStageMeshSet
        {
            get
            {
                if (this.pawn.ageTracker.CurLifeStageIndex == 1)
                {
                    return MeshPool.humanlikeBodySet;
                }
                if (this.pawn.story.crownType == CrownType.Narrow)
                {
                    return MeshPool.humanlikeHairSetNarrow;
                }
                Log.Error("Unknown crown type: " + this.pawn.story.crownType);
                return MeshPool.humanlikeHairSetAverage;
            }
        }

        public Pawn pawn;

    }
}
