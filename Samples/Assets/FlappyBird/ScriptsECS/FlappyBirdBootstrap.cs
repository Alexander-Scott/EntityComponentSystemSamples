using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlappyBird.ScriptsECS
{
    public sealed class FlappyBirdBootstrap
    {
        public static EntityArchetype BirdArchetype;
        public static EntityArchetype PipeArchetype;
        
        public static MeshInstanceRenderer BirdMesh;

        public static FlappyBirdSettings Settings { get; private set; }
        
        /// <summary>
        /// This method creates archetypes for entities we will spawn frequently in this game.
        /// Archetypes are optional but can speed up entity spawning substantially.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();
            
            // Create bird archetype
            BirdArchetype = entityManager.CreateArchetype(typeof(Position), typeof(PlayerInput));

            PipeArchetype = entityManager.CreateArchetype(typeof(Position));
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterSceneLoad()
        {
            var settingsGO = GameObject.Find("Settings");
            if (settingsGO == null)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }

            InitializeWithScene();
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            InitializeWithScene();
        }

        private static void InitializeWithScene()
        {
            var settingsGO = GameObject.Find("Settings");
            if (settingsGO == null)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }
            Settings = settingsGO?.GetComponent<FlappyBirdSettings>();
            if (!Settings)
                return;
            
            BirdMesh = GetLookFromPrototype("BirdRenderPrototype");
            
            NewGame();
        }

        /// <summary>
        /// Begin a new game
        /// </summary>
        public static void NewGame()
        {
            // Access the ECS entity manager
            EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();

            // Create an entity based on the bird archetype. It will get default-constructed
            // defaults for all the component types we listed.
            Entity bird = entityManager.CreateEntity(BirdArchetype);
            
            entityManager.SetComponentData(bird, new Position { Value = new float3(0,0,0) });            
            entityManager.AddSharedComponentData(bird, BirdMesh);
        }
        
        private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
        {
            var proto = GameObject.Find(protoName);
            var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
            Object.Destroy(proto);
            return result;
        }
    }
}
