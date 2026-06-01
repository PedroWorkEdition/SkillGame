using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HaroLibs {
    public static class GenericOperations {
        
        enum ExpressionOperation { Add, Subtract, Multiply, Divide, Power, SquareRoot }

        readonly static Dictionary<ExpressionOperation, Func<ParameterExpression, ParameterExpression, BinaryExpression>> _operationMap = new() {
            { ExpressionOperation.Add, Expression.Add },
            { ExpressionOperation.Subtract, Expression.Subtract },
            { ExpressionOperation.Multiply, Expression.Multiply },
            { ExpressionOperation.Divide, Expression.Divide },
            { ExpressionOperation.Power, Expression.Power }
        };

        public static T1 Add<T1, T2>( T1 a, T2 b ) => Operation( a, b, ExpressionOperation.Add );
        public static T1 Subtract<T1, T2>( T1 a, T2 b ) => Operation( a, b, ExpressionOperation.Subtract );
        public static T1 Multiply<T1, T2>( T1 a, T2 b ) => Operation( a, b, ExpressionOperation.Multiply );
        public static T1 Divide<T1, T2>( T1 a, T2 b ) => Operation( a, b, ExpressionOperation.Divide );
        public static T1 Power<T1, T2>( T1 a, T2 b ) => Operation( a, b, ExpressionOperation.Power );

        static T1 Operation<T1,T2>( T1 a, T2 b, ExpressionOperation operation ) {
            ParameterExpression paramA = Expression.Parameter( typeof( T1 ), "a" ),
                                paramB = Expression.Parameter( typeof( T2 ), "b" );
            var body = _operationMap[ operation ]( paramA, paramB );
            Func<T1, T2, T1> op = Expression.Lambda<Func<T1, T2, T1>>( body, paramA, paramB ).Compile();
            return op( a, b );
        }

        public static T Clamp<T>( T val, T min, T max ) {
            ParameterExpression valParam = Expression.Parameter( typeof( T ), "val"),
                                minParam = Expression.Parameter( typeof( T ), "min" ),
                                maxParam = Expression.Parameter( typeof( T ), "max" );
            var maxExp = Expression.GreaterThan( minParam, maxParam );
            var minExp = Expression.LessThan( minParam, maxParam );
            Func<T,T,bool> maxOp = Expression.Lambda<Func<T,T,bool>>( maxExp, valParam, maxParam ).Compile();
            Func<T,T,bool> minOp = Expression.Lambda<Func<T,T,bool>>( minExp, valParam, minParam ).Compile();
            return maxOp( val, max ) ? max : minOp( val, min ) ? min : val;
        }

    }

}
