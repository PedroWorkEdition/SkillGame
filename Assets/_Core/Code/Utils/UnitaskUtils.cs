using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace SkillGame.Utils {

    public static class UnitaskUtils {

        public static async UniTaskVoid WaitForSeconds( float seconds, Action onFinished, CancellationToken? token = null ) {
            CancellationTokenSource source = token == null ? new() : null;
            CancellationToken tk = token ?? source.Token;
            try {
                await UniTask.WaitForSeconds( seconds, cancellationToken: tk );
                onFinished?.Invoke();
            } catch {
                //UnityEngine.Debug.Log( "Canceled" );
            }
            source?.Cancel();
            source?.Dispose();
        }

        public static async UniTaskVoid WaitFixedFrame( Action onFinished, CancellationToken? token = null ) {
            CancellationTokenSource source = token == null ? new() : null;
            CancellationToken tk = token ?? source.Token;
            try {
                await UniTask.WaitForFixedUpdate();
                onFinished?.Invoke();
            } catch {
                //UnityEngine.Debug.Log( "Canceled" );
            }
            source?.Cancel();
            source?.Dispose();
        }


        public static async UniTaskVoid WaitOperation( UniTask operation, Action onFinished, CancellationToken? token = null ) {
            CancellationTokenSource source = token == null ? new() : null;
            CancellationToken tk = token ?? source.Token;
            try {
                await operation.AttachExternalCancellation( tk );
                onFinished?.Invoke();
            } catch {
                //UnityEngine.Debug.Log( "Canceled" );
            }
            source?.Cancel();
            source?.Dispose();
        }

        public static void DisposeToken( this CancellationTokenSource source ) {
            source?.Cancel();
            source?.Dispose();
        }

    }

}
