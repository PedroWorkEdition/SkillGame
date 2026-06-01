using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HaroLibs {
    public class PrefabPoolManager : PrivateLazySingleton<PrefabPoolManager> {

        Dictionary<PoolablePrefab, PoolContainer> _pools;
        Dictionary<GameObject, PoolContainer> _customPools;
        Dictionary<string, PoolContainer> _poolMap;

        protected override void Awake() {
            _pools = new();
            _customPools = new();
            _poolMap = new();
            base.Awake();
        }
        
        public static void CreatePool( PoolContainer container, GameObject origin = null ) {
            if (origin && !Instance._customPools.ContainsKey( origin )) {// i'll assume the user knows what it's doing
                Instance._customPools.Add( origin, container );
                Instance._poolMap.Add( container.ID, container );
                return;
            }
            if (Instance._pools.ContainsKey( container.Prefab )) return;
            if (!container.Parent) {
                var parent = new GameObject( $"{container.Prefab.name}_container" );
                parent.transform.SetParent( Instance.transform );
                container.Parent = parent.transform;
            }
            Instance._pools.Add( container.Prefab, container );
            Instance._poolMap.Add( container.ID, container );
        }

        public static PoolablePrefab GetItem( PoolablePrefab prefab ) =>
            Instance._pools.TryGetValue( prefab, out var container ) ? container.Get() : null;

        public static PoolablePrefab GetItem( GameObject origin ) =>
            Instance._customPools.TryGetValue( origin, out var container ) ? container.Get() : null;

        public static void ReleaseAll( PoolablePrefab prefab ) {
            if (!Instance._pools.ContainsKey( prefab )) return;
            Instance._pools[ prefab ].ReleaseAll();
        }

        public static void ReleaseAll( GameObject origin ) {
            if (!Instance._customPools.ContainsKey( origin )) return;
            Instance._customPools[ origin ].ReleaseAll();
        }

        public static void ReleaseItem( PoolablePrefab item ) {
            if (!Instance._poolMap.ContainsKey( item.ID )) return;
            Instance._poolMap[ item.ID ].Release( item );
        }
    }

    public class PoolContainer {

        public Transform Parent { get; internal set; }
        public readonly string ID;
        public readonly PoolablePrefab Prefab;
        readonly ObjectPool<PoolablePrefab> _pool;
        readonly List<PoolablePrefab> _activeObjs;

        Action<PoolablePrefab> _onCreate, _onGet, _onRelease, _onDestroy;

        internal PoolContainer( Transform parent, PoolablePrefab prefab, int minSize, int maxSize ) {
            ID = Guid.NewGuid().ToString();
            (Parent, Prefab) = ( parent, prefab );
            _activeObjs = new();
            _pool = new( CreateItem, GetItem, ReleaseItem, DestroyItem, defaultCapacity: minSize, maxSize: maxSize );
        }

        internal void SetCallbacks( Action<PoolablePrefab> onCreate, Action<PoolablePrefab> onGet,
                                    Action<PoolablePrefab> onRelease, Action<PoolablePrefab> onDestroy ) =>
                                    (_onCreate, _onGet, _onRelease, _onDestroy) = (onCreate, onGet, onRelease, onDestroy);

        PoolablePrefab CreateItem() {
            var obj = UnityEngine.Object.Instantiate( Prefab, Parent );
            obj.name = $"{obj.name}_{obj.transform.GetSiblingIndex()}";
            obj.SetID( ID );
            _onCreate?.Invoke( obj );
            return obj;
        }

        void GetItem( PoolablePrefab obj ) {
            _activeObjs.Add( obj );
            obj.OnPicked();
            _onGet?.Invoke( obj );
        }

        void ReleaseItem( PoolablePrefab obj ) {
            _activeObjs.Remove( obj );
            obj.OnReleased();
            _onRelease?.Invoke( obj );
        }

        void DestroyItem( PoolablePrefab obj ) { 
            _onDestroy?.Invoke( obj );
            UnityEngine.Object.Destroy( obj ); 
        }

        internal PoolablePrefab Get() => _pool.Get();
        internal void Release( PoolablePrefab obj ) => _pool.Release( obj );

        internal void ReleaseAll() { 
            var objs = _activeObjs.ToArray();
            objs.ForEach( obj => Release( obj ) );
        }

        public static PoolContainerBuilder Create( PoolablePrefab prefab ) => PoolContainerBuilder.Create( prefab );

    }

    public class PoolContainerBuilder {

        readonly PoolablePrefab _targetPrefab;
        Transform _parent;
        Action<PoolablePrefab> _onCreate, _onGet, _onRelease, _onDestroy;
        int _minSize, _maxSize;

        PoolContainerBuilder( PoolablePrefab targetPrefab  ) => _targetPrefab = targetPrefab;

        internal static PoolContainerBuilder Create( PoolablePrefab prefab ) => new ( prefab );
        public PoolContainerBuilder SetParent( Transform parent ) { _parent = parent; return this; }
        public PoolContainerBuilder RegisterOnCreateEvent( Action<PoolablePrefab> callback ) { _onCreate += callback; return this; }
        public PoolContainerBuilder RegisterOnGetEvent( Action<PoolablePrefab> callback ) { _onGet += callback; return this; }
        public PoolContainerBuilder RegisterOnReleaseEvent( Action<PoolablePrefab> callback ) { _onRelease += callback; return this; }
        public PoolContainerBuilder RegisterOnDestroyEvent( Action<PoolablePrefab> callback ) { _onDestroy += callback; return this; }
        public PoolContainerBuilder SetDefaultSize( int size ) { _minSize = size; return this; }
        public PoolContainerBuilder SetMaxSize( int size ) { _maxSize = size; return this; }

        public PoolContainer Build() {
            var container = new PoolContainer( _parent, _targetPrefab, _minSize, _maxSize );
            container.SetCallbacks( _onCreate, _onGet, _onRelease, _onDestroy );
            return container;
        }

    }

}
