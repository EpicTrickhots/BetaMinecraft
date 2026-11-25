using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace BetaMinecraft.Generator
{
    internal class ChunkGenerator(TerrainGenerator generator)
    {
        /// <summary>
        /// Radius of the rendered chunk circle around the viewer
        /// </summary>
        public const int RENDER_DISTANCE = 8;

        /// <summary>
        /// Maximum number of chunks that can be generated in a single chunk update
        /// </summary>
        public const int MAX_CHUNKS_PER_UPDATE = 2;

        /// <summary>
        /// Terrain generator that will handle mesh generation
        /// </summary>
        public TerrainGenerator Generator { get; } = generator;

        /// <summary>
        /// Chunk coordinate that the viewer is currently standing in
        /// </summary>
        private Vector3i viewerChunk = new Vector3i();

        /// <summary>
        /// All chunks that have been generated, active or not
        /// </summary>
        private readonly Dictionary<Vector3i, Chunk> cachedChunks = [];

        /// <summary>
        /// Chunks that have been generated, and are currently rendered
        /// </summary>
        private readonly List<Vector3i> renderedChunks = [];

        /// <summary>
        /// Chunks that are queued for rendering/generation
        /// </summary>
        private readonly Queue<Vector3i> queuedChunks = [];

        private Stopwatch debugTimer;

        public void Initialize()
        {
            Generator.CreateGpuResources();
        }

        public void CleanUp()
        {
            Generator.ReleaseGpuResources();
        }

        private void CreateChunk(Vector3i coordinate)
        {
            Chunk chunk = new();
        }
    }
}
