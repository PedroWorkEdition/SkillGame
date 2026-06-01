using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HaroLibs;

namespace HaroLibsEditor {
    public static class SearchWindowHelper {

        const char k_genericSymbol = '`';

        public static SearchWindowUtil CreateTypeCategorizedCreationSearchWindow<T>( Action<Type> itemCreation = null ) {
            var types = AssemblyTypeDefinitions.GetAllTypes<T>();
            var searchWindow = ScriptableObject.CreateInstance<SearchWindowUtil>();
            var baseType = typeof( T );
            var inheritanceMap = new Dictionary<Type, HashSet<Type>>();
            SetupInheritanceMap( inheritanceMap, types, baseType );
            searchWindow.Initialize<Type>( new( t => itemCreation( t ) ) );
            searchWindow.SetEntries( types.Select( t => new SearchEntryContext( 1, t.Name, t ) ).ToArray() );
            searchWindow.SetGroups( GetSearchGroups( inheritanceMap, baseType ) );
            return searchWindow;
        }

        static void SetupInheritanceMap( Dictionary<Type, HashSet<Type>> map, List<Type> types, Type baseType ) {
            List<Type> toRemove = new ();
            foreach (var type in types) {
                if (type.BaseType == baseType || type.BaseType == typeof( object )) continue;
                AddToMap( map, type, baseType );
                toRemove.Add( type );
            }
            ClearRedundantInheretance( map, toRemove, types );
        }

        static void AddToMap( Dictionary<Type, HashSet<Type>> map, Type targetType, Type baseType ) {
            for (var type = targetType; TypeCheck(type, baseType); type = type.BaseType) {
                var currentBase = type.BaseType;
                if (currentBase.IsGenericType)
                    currentBase = currentBase.GetGenericTypeDefinition();
                if (!map.ContainsKey( currentBase )) 
                    map.Add( currentBase, new HashSet<Type>() );
                if (type.IsAbstract) continue;
                map[ currentBase ].Add( type );
            }
        }

        static void ClearRedundantInheretance( Dictionary<Type, HashSet<Type>> map, List<Type> types, List<Type> source ) {
            foreach (var type in types) {
                if (map.ContainsKey( type )) {
                    map[ type ].Add( type );
                    if ( map.ContainsKey( type.BaseType ) )
                        map[ type.BaseType ].Remove( type );
                }
                source.Remove( type );
            }
        }

        static bool TypeCheck( Type current, Type baseType ) =>
                                baseType.IsInterface ? baseType.IsAssignableFrom( current ) && current.BaseType != typeof( object ) :
                                current.BaseType == baseType;

        static SearchGroupEntry[] GetSearchGroups( Dictionary<Type, HashSet<Type>> map, Type baseType ) {
            var floorTypes = map.Keys.Where( key => !baseType.IsAssignableFrom( key.BaseType ) ).ToArray();
            //floorTypes.ForEach( key => Debug.Log( key ) );
            //map.Keys.ForEach( key => Debug.Log( $"current: {key}, base: {key.BaseType}" ) );
            
            List<SearchGroupEntry> groups = new();
            foreach ( var floorType in floorTypes ) {
                var hierarchyList = map.Keys.Where( key => floorType.IsAssignableFrom( key ) )
                                            .OrderBy( key => key.GetInheritanceHierarchy( baseType ).Count() )
                                            .ToList();
                foreach ( var item in hierarchyList ) 
                    groups.Add( GetGroup( item ) );
            }
            //var groups = map.Keys.Select( t => GetGroup( t ) ).ToArray();
            //return groups;
            return groups.ToArray();
            SearchGroupEntry GetGroup( Type t ) {
                string name = FilterEntryName( t.Name );
                int depth = t.GetInheritanceHierarchy( baseType ).Count();
                var entries = map[ t ].Select( sub => GetEntry( sub ) ).ToArray();
                //Debug.Log( $"Group of type: {t}, name: {name}, depth: {depth}, children: {entries.Length}" );
                return new SearchGroupEntry( name, depth, entries );
            }
            SearchEntryContext GetEntry( Type sub ) {
                string name = sub.Name;
                int depth = sub.GetInheritanceHierarchy( baseType ).Count() + ( map.ContainsKey( sub ) ? 1 : 0 );
                //Debug.Log( $"Entry of type: {sub}, Name: {name}, depth: {depth}" );
                return new SearchEntryContext( depth, name, sub );
            }
        }

        static string FilterEntryName( string name ) {
            if (name.Contains( k_genericSymbol ))
                return name[0..^2].ToString();
            return name;
        }

        public static SearchWindowUtil CreateTypeCreationSearchWindow<T>( Action<Type> itemCreation = null ) {
            var types = AssemblyTypeDefinitions.GetAllTypes<T>();
            var searchWindow = ScriptableObject.CreateInstance<SearchWindowUtil>();
            searchWindow.Initialize<Type>( new( t => itemCreation( t ) ) );
            searchWindow.SetEntries( types.Select( t => new SearchEntryContext( 1, t.Name, t ) ).ToArray() );
            return searchWindow;
        }
    }
}
